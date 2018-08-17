using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Website.Data.EF.Models;
using Website.Service.DTO;
using Website.Service.Enums;
using Website.Service.IdentityStores;
using Website.Service.Infrastructure;
using Website.Service.Interfaces;

namespace Website.Service.Services
{
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

        private readonly DbContext _dbContext;
        private readonly IMapper _mapper;

        /// <summary>
        /// Creates or updates profile of user with certain login (UserName)
        /// </summary>
        /// <param name="userProfileDto"></param>
        /// <returns></returns>
        public async Task<OperationDetails> CreateOrUpdateProfileAsync(UserProfileDTO userProfileDto)
        {
            this.ThrowIfDisposed();

            if (userProfileDto?.Login == null) return new OperationDetails(false, "Некорректный профиль.", nameof(userProfileDto));


            var userDbSet = _dbContext.Set<User>();

            var user = await userDbSet.Where(x => x.NormalizedUserName == userProfileDto.Login.ToUpper()).Include(x => x.UserProfile).FirstOrDefaultAsync();
            if (user == null)
                return new OperationDetails(false, "Пользователь с таким логином не найден.", nameof(userProfileDto.Login));

            OperationDetails opDetails;
            var clProfile = user.UserProfile;
            var profileDbSet = _dbContext.Set<UserProfile>();
            if (clProfile != null)
            {
                clProfile = _mapper.Map<UserProfile>(userProfileDto);
                profileDbSet.Update(clProfile);
                opDetails = new OperationDetails(true, "Профиль успешно изменен.", "");
            }
            else
            {
                clProfile = _mapper.Map<UserProfile>(userProfileDto);
                clProfile.Id = user.Id;
                profileDbSet.Add(clProfile);
                opDetails = new OperationDetails(true, "Профиль успешно создан.", "");
            }

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                Logger.Log(LogLevel.Error, e, "Возникла ошибка при обновлении профиля клиента.");
                return new OperationDetails(false, "Возникла ошибка при обновлении профиля.", "");
            }

            return opDetails;
        }

        public async Task<IEnumerable<UserDTO>> GetUsersAsync(RoleSelector rolePick, int skip, int take)
        {
            this.ThrowIfDisposed();

            var query = GetUsersByRoleAsQuery(rolePick);

            List<User> users = await query.Skip(skip).Take(take).ToListAsync();

            var clientList = new List<UserDTO>();
            foreach (var user in users)
            {
                var client = _mapper.Map<UserDTO>(user);
                clientList.Add(client);
            }

            return clientList;
        }

        private IQueryable<User> GetUsersByRoleAsQuery(RoleSelector rolePick)
        {
            this.ThrowIfDisposed();

            var userSet = _dbContext.Set<User>();
            var roleSet = _dbContext.Set<Role>();
            var userRoleSet = _dbContext.Set<UserRole>();

            var anonymQuery = userRoleSet
                .Join(userSet, userRole => userRole.UserId, user => user.Id, (userRole, user) => new { user, userRole })
                .Join(roleSet, userToUserRole => userToUserRole.userRole.RoleId, role => role.Id, (userToUserRole, role) => new { userToUserRole.user, role });

            IQueryable<User> finalQuery;

            switch (rolePick)
            {
                case RoleSelector.Administrators:
                    finalQuery = anonymQuery
                        .Where(x => x.role.Name == "admin" || x.role.Name == "manager")
                        .Select(x => x.user)
                        .Include(x => x.UserProfile);
                    break;

                case RoleSelector.Clients:
                    finalQuery = anonymQuery
                        .Where(x => x.role.Name == "user")
                        .Select(x => x.user)
                        .Include(x => x.UserProfile);
                    break;

                default:
                    finalQuery = userSet;
                    break;
            }

            finalQuery = finalQuery
                .Include(x => x.Claims)
                .Include(x => x.UserProfile);

            return finalQuery;

        }
        public async Task<SortPageResult<UserDTO>> GetSortFilterPageAsync(RoleSelector roleSelector, string searchString, string sortPropName, int page, int pageCount)
        {
            //TODO refactor into smaller methods

            // check inputs
            if (sortPropName == null) throw new ArgumentNullException(nameof(sortPropName));
            if (pageCount < 0) throw new ArgumentOutOfRangeException(nameof(pageCount));
            if (page < 0) throw new ArgumentOutOfRangeException(nameof(page));
            if (!Enum.IsDefined(typeof(RoleSelector), roleSelector))
                throw new InvalidEnumArgumentException(nameof(roleSelector), (int)roleSelector, typeof(RoleSelector));

            bool descending = false;
            if (sortPropName.EndsWith("_desc"))
            {
                sortPropName = sortPropName.Substring(0, sortPropName.Length - 5);
                descending = true;
            }
            // checking property name
            string propClass = "";
            var result = CheckIfPropertyExists(sortPropName, typeof(UserDTO), typeof(UserProfileDTO));
            if (result.Result)
            {
                propClass = result.Type.Name;
            }

            // filetring
            var query = GetUsersByRoleAsQuery(roleSelector);

            // searching
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

            // ordering

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

            // paginating
            var totalUsers = await query.CountAsync();
            query = query.Skip((page - 1) * pageCount).Take(pageCount);

            var usersDto = await query.ProjectTo<UserDTO>(_mapper.ConfigurationProvider).ToListAsync();

            return new SortPageResult<UserDTO> { FilteredData = usersDto, TotalN = totalUsers };
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
                Logger.LogError("Попытка зарегистрировать активносить несущуствующего пользователя");
                return;
            }
            user.LastActivityDate = DateTimeOffset.Now;
            set.Update(user);
            await _dbContext.SaveChangesAsync();
        }

        #region helpers

        private PropertyCheckResult CheckIfPropertyExists(string sortPropName, params Type[] types)
        {
            foreach (var type in types)
            {
                var typeProps = type.GetProperties(System.Reflection.BindingFlags.Public
                                                              | System.Reflection.BindingFlags.Instance
                                                              | System.Reflection.BindingFlags.DeclaredOnly)
                    .Select(x => x.Name).ToArray();
                if (typeProps.Contains(sortPropName))
                    return new PropertyCheckResult(true, type);
            }
            return new PropertyCheckResult(false);
        }

        private class PropertyCheckResult
        {
            public Type Type;
            public bool Result;

            public PropertyCheckResult(bool result, Type type = null)
            {
                Type = type;
                Result = result;
            }
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