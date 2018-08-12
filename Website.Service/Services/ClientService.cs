using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Website.Data.EF.Models;
using Website.Service.DTO;
using Website.Service.Infrastructure;
using Website.Service.Interfaces;
using Website.Service.Mapper;
using ClientProfile = Website.Data.EF.Models.ClientProfile;

namespace Website.Service.Services
{
    //TODO tests
    //TODO concurrency checks 
    public class ClientService : IClientService, IDisposable
    {
        private DbContext _context;
        //private UserManager<ApplicationUser> _userManager;
        private ILogger _logger;
        private IMapper _mapper;

        public ClientService(DbContext context, /*UserManager<ApplicationUser> userManager,*/ ILogger<ClientService> logger)
        {
            _context = context;
            //_userManager = userManager;
            _logger = logger;
           
            _mapper = new MapperConfiguration(cfg=>
            {
                cfg.AddProfile<ClientProfileMapperProfile>();
                cfg.AddProfile<ClientMapperProfile>();
            }).CreateMapper();
        }
        public async Task<OperationDetails> CreateOrUpdateProfileAsync(ClientProfileDTO clientProfileDto)
        {
            this.ThrowIfDisposed();

            if (clientProfileDto?.Email == null)
                return new OperationDetails(false, "Некорректная модель.", nameof(clientProfileDto));

            //var user = await _userManager.FindByEmailAsync(clientProfileDto.Email);
            var userDbSet = _context.Set<ApplicationUser>();
            var user = await userDbSet.Where(x => x.NormalizedEmail == clientProfileDto.Email.ToUpper()).Include(x => x.ClientProfile).FirstOrDefaultAsync();
            if (user == null)
                return new OperationDetails(false, "Пользователь с таким e-mail не найден.", nameof(clientProfileDto.Email));

            OperationDetails opDetails;
            var clProfile = user.ClientProfile;
            var profileDbSet = _context.Set<ClientProfile>();
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
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                _logger.Log(LogLevel.Error, e, "Возникла ошибка при обновлении профиля клиента.");
                return new OperationDetails(false, "Возникла ошибка при обновлении профиля.", "");
            }

            return opDetails;
        }

        public async Task<IEnumerable<ClientDTO>> GetUsersAsync()
        {
            this.ThrowIfDisposed();

            var set = _context.Set<ApplicationUser>();
            var users = await set
                .Include(x => x.ClientProfile)
                .Include(x=>x.Claims)
                .ToListAsync();

            var clientList = new List<ClientDTO>();
            var client = new ClientDTO();
            foreach (var user in users)
            {
                client = _mapper.Map<ClientDTO>(user);
                clientList.Add(client);
            }

            return clientList;
        }

        //public async Task<string> GetFullName(string email)
        //{
        //    this.ThrowIfDisposed();

        //    if (email == null)
        //        return null;

        //    var user = await _userManager.FindByEmailAsync(email);
        //    var fullName = user?.ClientProfile?.FullName;
        //    if (fullName?.Length > 3)
        //        return fullName;

        //    return null;
        //}


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
                    _context?.Dispose();
                }

                // free unmanaged resources (unmanaged objects) and override a finalizer below.
                // set large fields to null.

                _disposed = true;
            }
        }

        // override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ClientService() {
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
