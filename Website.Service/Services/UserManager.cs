using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
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
    public class UserManager : UserManager<UserDTO>, IUserManager
    {
        public UserManager(IUserStore<UserDTO> store,
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
        }

        private DbContext DbContext => (Store as CustomUserStore)?.Context;
        private readonly IMapper _mapper;

        //public override Task<IdentityResult> CreateAsync(UserDTO user)
        //{
        //    user.Id = Guid.NewGuid().ToString();
        //    return base.CreateAsync(user);
        //}

        public async Task<OperationDetails> CreateOrUpdateProfileAsync(UserProfileDTO userProfileDto)
        {
            this.ThrowIfDisposed();

            if (userProfileDto?.Email == null)return new OperationDetails(false, "Некорректная модель.", nameof(userProfileDto));


            var userDbSet = DbContext.Set<User>();
            var user = await userDbSet.Where(x => x.NormalizedEmail == userProfileDto.Email.ToUpper()).Include(x => x.UserProfile).FirstOrDefaultAsync();
            if (user == null)
                return new OperationDetails(false, "Пользователь с таким e-mail не найден.", nameof(userProfileDto.Email));

            OperationDetails opDetails;
            var clProfile = user.UserProfile;
            var profileDbSet = DbContext.Set<UserProfile>();
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
                await DbContext.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                Logger.Log(LogLevel.Error, e, "Возникла ошибка при обновлении профиля клиента.");
                return new OperationDetails(false, "Возникла ошибка при обновлении профиля.", "");
            }

            return opDetails;
        }

        public async Task<ICollection<UserDTO>> GetUsersAsync(RoleSelector rolePick, int skip, int take)
        {
            this.ThrowIfDisposed();

            var query = GetUsersAsQuery(rolePick, skip, take);

            List<User> users = await query.ToListAsync();

            var clientList = new List<UserDTO>();
            foreach (var user in users)
            {
                var client = _mapper.Map<UserDTO>(user);
                clientList.Add(client);
            }

            return clientList;
        }

        private IQueryable<User> GetUsersAsQuery(RoleSelector rolePick, int skip, int take)
        {
            this.ThrowIfDisposed();

            var userSet = DbContext.Set<User>();
            var roleSet = DbContext.Set<Role>();
            var userRoleSet = DbContext.Set<UserRole>();

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
                .Skip(skip)
                .Take(take)
                .Include(x => x.Claims)
                .Include(x => x.UserProfile);

            return finalQuery;

        }
        public async Task<ICollection<UserDTO>> GetSortFilterPageAsync(string sortPropName, string currentFilter, string searchString, int? page, int? count)
        {
            await Task.CompletedTask;
            return new List<UserDTO>();
        }
        /// <summary>
        /// Сохраняет в бд текущее время как дату последней активности (LastActivityDate)
        /// </summary>
        /// <param name="userLogin">Логин пользователя</param>
        /// <returns></returns>
        public async Task LogUserActivity(string userLogin)
        {
            this.ThrowIfDisposed();

            var set = DbContext.Set<User>();
            var user = set.FirstOrDefault(x => x.UserName == userLogin);
            if (user == null)
            {
                Logger.LogError("Попытка зарегистрировать активносить несущуствующего пользователя");
                return;
            }
            user.LastActivityDate = DateTimeOffset.Now;
            set.Update(user);
            await DbContext.SaveChangesAsync();
        }

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