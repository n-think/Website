using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Castle.Core.Internal;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Website.Data.EF.Models;
using Website.Service.DTO;
using Website.Service.Enums;
using Website.Service.Infrastructure;
using Website.Service.Interfaces;
using Website.Service.Stores;

namespace Website.Service.Services
{
    //TODO refactor using store (then remove mapper, dbcontext)

    public class UserManager : AspNetUserManager<UserDTO>, IUserManager
    {
        public UserManager(
            DbContext context,
            IUserStore<UserDTO> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<UserDTO> passwordHasher,
            IEnumerable<IUserValidator<UserDTO>> userValidators,
            IEnumerable<IPasswordValidator<UserDTO>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<UserManager<UserDTO>> logger,
            IMapper mapper)
            : base(store,
                optionsAccessor,
                passwordHasher,
                userValidators,
                passwordValidators,
                keyNormalizer,
                errors,
                services,
                logger)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _dbContext = context;
        }

        private readonly DbContext _dbContext; // DI scoped context
        private readonly IMapper _mapper; //DI

        /// <summary>
        /// Creates or updates profile of user with certain login (UserName)
        /// </summary>
        /// <param name="userProfileDto"></param>
        /// <returns></returns>
        public async Task<OperationResult> CreateOrUpdateProfileAsync(UserProfileDTO userProfileDto)
        {
            this.ThrowIfDisposed();

            if (userProfileDto?.Login == null) return OperationResult.Failure(new OperationError() { Description = "Некорректный профиль." });

            var userDbSet = _dbContext.Set<User>();

            var user = await userDbSet.Where(x => x.NormalizedUserName == userProfileDto.Login.ToUpper()).Include(x => x.UserProfile).FirstOrDefaultAsync();
            if (user == null)
                return OperationResult.Failure(new OperationError() { Description = "Пользователь с таким логином не найден." });

            OperationResult opResult;
            var clProfile = user.UserProfile;
            var profileDbSet = _dbContext.Set<UserProfile>();
            if (clProfile != null)
            {
                clProfile = _mapper.Map<UserProfile>(userProfileDto);
                profileDbSet.Update(clProfile);
                opResult = OperationResult.Success();
            }
            else
            {
                clProfile = _mapper.Map<UserProfile>(userProfileDto);
                clProfile.Id = user.Id;
                profileDbSet.Add(clProfile);
                opResult = OperationResult.Success();
            }

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            { //TODO
                Logger.Log(LogLevel.Error, e, "Возникла ошибка при обновлении профиля клиента."); 
                return OperationResult.Failure(new OperationError() { Description = "Возникла ошибка при обновлении профиля." });
            }

            return opResult;
        }

        public async Task<IEnumerable<UserDTO>> GetUsersAsync(RoleSelector roleSelector, int skip, int take)
        {
            this.ThrowIfDisposed();
            // check inputs
            if (!Enum.IsDefined(typeof(RoleSelector), roleSelector))
                throw new InvalidEnumArgumentException(nameof(roleSelector), (int)roleSelector, typeof(RoleSelector));
            if (skip < 0) throw new ArgumentOutOfRangeException(nameof(skip));
            if (take <= 0) throw new ArgumentOutOfRangeException(nameof(take));

            var query = GetUsersByRoleAsQuery(roleSelector);

            List<User> users = await query.Skip(skip).Take(take).ToListAsync();

            var clientList = new List<UserDTO>();
            foreach (var user in users)
            {
                var client = _mapper.Map<UserDTO>(user);
                clientList.Add(client);
            }

            return clientList;
        }


        public async Task<SortPageResult<UserDTO>> GetSortFilterPageAsync(RoleSelector roleSelector, string searchString, string sortPropName, int page, int pageCount)
        {
            ThrowIfDisposed();
            // check inputs
            if (sortPropName == null) throw new ArgumentNullException(nameof(sortPropName));
            if (pageCount < 0) throw new ArgumentOutOfRangeException(nameof(pageCount));
            if (page < 0) throw new ArgumentOutOfRangeException(nameof(page));
            if (!Enum.IsDefined(typeof(RoleSelector), roleSelector))
                throw new InvalidEnumArgumentException(nameof(roleSelector), (int)roleSelector, typeof(RoleSelector));

            // filter roles
            IQueryable<User> query = GetUsersByRoleAsQuery(roleSelector);

            // searching
            SearchUsersQuery(searchString, ref query);

            // ordering
            OrderUserQuery(sortPropName, ref query);

            // paginating
            var totalUsers = await query.CountAsync();
            query = query.Skip((page - 1) * pageCount).Take(pageCount);

            var usersDto = await query.ProjectTo<UserDTO>(_mapper.ConfigurationProvider).ToListAsync(CancellationToken);

            return new SortPageResult<UserDTO> { FilteredData = usersDto, TotalN = totalUsers };
        }

        private void OrderUserQuery(string sortPropName, ref IQueryable<User> query)
        {
            this.ThrowIfDisposed();
            if (sortPropName.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(sortPropName));

            bool descending = false;
            if (sortPropName.EndsWith("_desc"))
            {
                sortPropName = sortPropName.Substring(0, sortPropName.Length - 5);
                descending = true;
            }

            // checking property name
            string propClass = "";
            var check = StoreHelpers.CheckIfPropertyExists(sortPropName, typeof(UserDTO), typeof(UserProfileDTO));
            if (check.Result)
            {
                propClass = check.Type.Name;
            }
            else
                throw new ArgumentNullException(nameof(sortPropName));

            Expression<Func<User, object>> userValue = u => EF.Property<object>(u, sortPropName);
            Expression<Func<User, object>> profileValue = u => EF.Property<object>(u.UserProfile, sortPropName);
            switch (propClass)
            {
                case nameof(UserDTO):
                    if (descending)
                        query = query.OrderByDescending(userValue);
                    else
                        query = query.OrderBy(userValue);
                    break;

                case nameof(UserProfileDTO):
                    if (descending)
                        query = query.OrderByDescending(profileValue);
                    else
                        query = query.OrderBy(profileValue);
                    break;
                default:
                    query = query.OrderBy(x => x.UserName);
                    break;
            }
        }

        private IQueryable<User> GetUsersByRoleAsQuery(RoleSelector rolePick)
        {
            this.ThrowIfDisposed();

            var userSet = _dbContext.Set<User>();
            var roleSet = _dbContext.Set<Role>();
            var userRoleSet = _dbContext.Set<UserRole>();

            var anonymQuery = userRoleSet // можно проще с нав свойствами которые я добавил позже чем написал это
                .Join(userSet, userRole => userRole.UserId, user => user.Id, (userRole, user) => new { user, userRole })
                .Join(roleSet, userToUserRole => userToUserRole.userRole.RoleId, role => role.Id, (userToUserRole, role) => new { userToUserRole.user, role });

            IQueryable<User> finalQuery;

            switch (rolePick)
            {
                case RoleSelector.Administrators:
                    finalQuery = anonymQuery
                        .Where(x => x.role.Name == "admin")
                        .Select(x => x.user);

                    break;

                case RoleSelector.Users:
                    finalQuery = anonymQuery
                        .Where(x => x.role.Name == "user")
                        .Select(x => x.user);
                    break;

                default:
                    finalQuery = anonymQuery
                        .Where(x => x.role.Name == "user" || x.role.Name == "admin")
                        .Select(x => x.user);
                    break;
            }

            finalQuery = finalQuery
                .Include(x => x.Claims)
                .Include(x => x.UserProfile);

            return finalQuery;
        }

        private IQueryable<User> SearchUsersQuery(string searchString, ref IQueryable<User> query)
        {
            this.ThrowIfDisposed();

            if (!String.IsNullOrEmpty(searchString))
            {
                query = query.Where(x => x.UserName.Contains(searchString) ||
                                         x.Email.Contains(searchString) ||
                                         x.PhoneNumber != null && x.PhoneNumber.Contains(searchString) ||
                                         x.UserProfile.FirstName != null && x.UserProfile.FirstName.Contains(searchString) ||
                                         x.UserProfile.LastName != null && x.UserProfile.LastName.Contains(searchString) ||
                                         x.UserProfile.PatrName != null && x.UserProfile.PatrName.Contains(searchString)
                );
            }

            return query;
        }

        /// <summary>
        /// Сохраняет в бд текущее время как дату последней активности (LastActivityDate)
        /// </summary>
        /// <param name="userLogin">Логин пользователя</param>
        /// <returns></returns>
        public async Task LogUserActivity(string userLogin)
        {
            this.ThrowIfDisposed();

            var set = _dbContext.Set<User>();
            var user = set.FirstOrDefault(x => x.UserName == userLogin);
            if (user == null)
            {
                //Logger.LogError("Попытка зарегистрировать активность несущeствующего пользователя"); //lul
                return;
            }
            user.LastActivityDate = DateTimeOffset.Now;
            set.Update(user);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Updates user with new password and claims, ignores (!) claims values.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="newPassword"></param>
        /// <param name="newClaims"></param>
        /// <returns></returns>
        public async Task<IdentityResult> UpdateUserPasswordClaims(UserDTO user, string newPassword, IEnumerable<Claim> newClaims)
        {
            //TODO tests?
            //TODO refactor into smaller methods

            if (user == null) throw new ArgumentNullException(nameof(user));
            ThrowIfDisposed();

            //update password
            if (!newPassword.IsNullOrEmpty())
            {
                var passwordResult = await SetPasswordAsync(user, newPassword);
                if (!passwordResult.Succeeded)
                {
                    return passwordResult;
                }
            }

            if (!newClaims.IsNullOrEmpty())
            {
                #region update roles

                if (!(Store is IUserRoleStore<UserDTO> roleStore))
                {
                    throw new NotSupportedException("Current UserStore doesn't implement IUserRoleStore");
                }

                var roles = newClaims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value);
                var userRoleList = await roleStore.GetRolesAsync(user, CancellationToken);
                var compResult = StoreHelpers.ChangeCompare(userRoleList, roles, x => x);
                var rolesToRemove = compResult.Deleted;
                var rolesToAdd = compResult.Inserted.Distinct();

                foreach (var role in rolesToRemove)
                {
                    var normalizedRole = NormalizeKey(role);
                    await roleStore.RemoveFromRoleAsync(user, normalizedRole, CancellationToken);
                }
                foreach (var role in rolesToAdd)
                {
                    var normalizedRole = NormalizeKey(role);
                    if (!await roleStore.IsInRoleAsync(user, normalizedRole, CancellationToken))
                    {
                        await roleStore.AddToRoleAsync(user, normalizedRole, CancellationToken);
                    }
                }

                #endregion
                #region update claims

                if (!(Store is IUserClaimStore<UserDTO> claimStore))
                {
                    throw new NotSupportedException("Current UserStore doesn't implement IUserClaimStore");
                }

                var claims = newClaims.Where(x => x.Type != ClaimTypes.Role).ToList();
                var userClaims = await GetClaimsAsync(user);
                var claimCompResult = StoreHelpers.ChangeCompare(userClaims, claims, x => x.Type);
                var claimsToRemove = claimCompResult.Deleted;
                var claimsToAdd = claimCompResult.Inserted.Distinct();

                await claimStore.RemoveClaimsAsync(user, claimsToRemove, CancellationToken);
                await claimStore.AddClaimsAsync(user, claimsToAdd, CancellationToken);

                #endregion
            }

            //update user return result
            return await UpdateAsync(user);
        }

        public async Task<UserProfileDTO> FindProfileByUserIdAsync(string userId)
        {
            ThrowIfDisposed();
            if (userId.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(userId));
            var profileStore = Store as IUserProfileStore<UserProfileDTO>;
            if (profileStore == null)
            {
                throw new NotSupportedException("Current UserStore doesn't implement IUserProfileStore");
            }
            return await profileStore.FindProfileByUserIdAsync(userId, CancellationToken);
        }

        #region helpers

        private async Task<IdentityResult> SetPasswordAsync(UserDTO user, string newPassword)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            var passwordStore = Store as IUserPasswordStore<UserDTO>;
            if (passwordStore == null)
            {
                var error = new IdentityError
                {
                    Description = "Current UserStore doesn't implement IUserPasswordStore"
                };
                return IdentityResult.Failed(error);
            }

            var passwordValidateResult = await ValidatePasswordAsync(user, newPassword);
            if (!passwordValidateResult.Succeeded)
            {
                return passwordValidateResult;
            }

            var newPasswordHash = this.PasswordHasher.HashPassword(user, newPassword);
            await passwordStore.SetPasswordHashAsync(user, newPasswordHash, CancellationToken);
            await UpdateSecurityStampAsync(user);
            return IdentityResult.Success;
        }

        #endregion


        #region IDisposable Support
        private bool _disposed = false; // To detect redundant calls

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!_disposed)
            {
                // free unmanaged resources (unmanaged objects) and override a finalizer below.
                // set large fields to null.

                _disposed = true;
            }
        }
        // override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~UserManager() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        #endregion
    }
}