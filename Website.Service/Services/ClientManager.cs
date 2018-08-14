using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Website.Data.EF.Models;
using Website.Service.DTO;
using Website.Service.Enums;
using Website.Service.Infrastructure;
using Website.Service.Interfaces;
using Website.Service.Mapper;
using ClientProfile = Website.Data.EF.Models.ClientProfile;

namespace Website.Service.Services
{
    //TODO tests
    //TODO concurrency checks 
    public class ClientManager : IClientManager, IDisposable
    {
        private DbContext _dbContext;
        //private UserManager<ApplicationUser> _userManager;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClientManager(DbContext dbContext, /*UserManager<ApplicationUser> userManager,*/ ILogger<ClientManager> logger, IHttpContextAccessor httpContextAccessorAccessor)
        {
            _dbContext = dbContext;
            //_userManager = userManager;
            _logger = logger;
            _httpContextAccessor = httpContextAccessorAccessor;
            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MapperProfile>();
            }).CreateMapper();
        }
        public async Task<OperationDetails> CreateOrUpdateProfileAsync(ClientProfileDTO clientProfileDto)
        {
            this.ThrowIfDisposed();

            if (clientProfileDto?.Email == null)
                return new OperationDetails(false, "Некорректная модель.", nameof(clientProfileDto));

            //var user = await _userManager.FindByEmailAsync(clientProfileDto.Email);
            var userDbSet = _dbContext.Set<ApplicationUser>();
            var user = await userDbSet.Where(x => x.NormalizedEmail == clientProfileDto.Email.ToUpper()).Include(x => x.ClientProfile).FirstOrDefaultAsync();
            if (user == null)
                return new OperationDetails(false, "Пользователь с таким e-mail не найден.", nameof(clientProfileDto.Email));

            OperationDetails opDetails;
            var clProfile = user.ClientProfile;
            var profileDbSet = _dbContext.Set<ClientProfile>();
            if (clProfile != null)
            {
                clProfile = _mapper.Map<ClientProfile>(clientProfileDto);
                profileDbSet.Update(clProfile);
                opDetails = new OperationDetails(true, "Профиль успешно изменен.", "");
            }
            else
            {
                clProfile = _mapper.Map<ClientProfile>(clientProfileDto);
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
                _logger.Log(LogLevel.Error, e, "Возникла ошибка при обновлении профиля клиента.");
                return new OperationDetails(false, "Возникла ошибка при обновлении профиля.", "");
            }

            return opDetails;
        }

        public async Task<ICollection<ClientDTO>> GetUsersAsync(RoleSelector rolePick, int skip, int take)
        {
            this.ThrowIfDisposed();

            var query = GetUsersAsQuery(rolePick, skip, take);

            List<ApplicationUser> users = await query.ToListAsync();

            var clientList = new List<ClientDTO>();
            foreach (var user in users)
            {
                var client = _mapper.Map<ClientDTO>(user);
                clientList.Add(client);
            }

            return clientList;
        }

        private IQueryable<ApplicationUser> GetUsersAsQuery(RoleSelector rolePick, int skip, int take)
        {
            this.ThrowIfDisposed();

            var userSet = _dbContext.Set<ApplicationUser>();
            var roleSet = _dbContext.Set<IdentityRole>();
            var userRoleSet = _dbContext.Set<IdentityUserRole<string>>();

            var anonymQuery = userRoleSet
                .Join(userSet, userRole => userRole.UserId, user => user.Id, (userRole, user) => new { user, userRole })
                .Join(roleSet, userToUserRole => userToUserRole.userRole.RoleId, role => role.Id, (userToUserRole, role) => new { userToUserRole.user, role });

            IQueryable<ApplicationUser> finalQuery;

            switch (rolePick)
            {
                case RoleSelector.Administrators:
                    finalQuery = anonymQuery
                        .Where(x => x.role.Name == "admin" || x.role.Name == "manager")
                        .Select(x => x.user)
                        .Include(x => x.ClientProfile);
                    break;

                case RoleSelector.Clients:
                    finalQuery = anonymQuery
                        .Where(x => x.role.Name == "user")
                        .Select(x => x.user)
                        .Include(x => x.ClientProfile);
                    break;

                default:
                    finalQuery = userSet;
                    break;
            }

            finalQuery = finalQuery
                .Skip(skip)
                .Take(take)
                .Include(x => x.Claims)
                .Include(x => x.ClientProfile);

            return finalQuery;

        }
        public async Task<ICollection<ClientDTO>> GetSortFilterPageAsync(string sortPropName, string currentFilter, string searchString, int? page, int? count)
        {
            await Task.CompletedTask;
            return new List<ClientDTO>();
        }
        /// <summary>
        /// Сохраняет в бд текущее время как дату последней активности (LastActivityDate)
        /// </summary>
        /// <param name="userLogin">Логин пользователя</param>
        /// <returns></returns>
        public async Task LogUserActivity(string userLogin)
        {
            this.ThrowIfDisposed();

            var set = _dbContext.Set<ApplicationUser>();
            var user = set.FirstOrDefault(x => x.UserName == userLogin);
            if (user == null)
            {
                _logger.LogError("Попытка зарегистрировать активносить несущуствующего пользователя");
                return;
            }
            user.LastActivityDate = DateTimeOffset.Now;
            set.Update(user);
            await _dbContext.SaveChangesAsync();
        }

        protected void ThrowIfDisposed()
        {
            if (this._disposed)
                throw new ObjectDisposedException(this.GetType().Name);
        }

        #region IDisposable Support
        private bool _disposed = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _dbContext?.Dispose();
                }

                // free unmanaged resources (unmanaged objects) and override a finalizer below.
                // set large fields to null.

                _disposed = true;
            }
        }

        // override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ClientManager() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        #endregion
    }
}
