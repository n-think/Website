using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Website.Data.EF.Models;
using Website.Service.DTO;
using Website.Service.Infrastructure;
using Website.Service.Interfaces;

namespace Website.Service.Services
{
    //TODO concurrency checks (need client profile field first)
    public class ClientProfileService : IClientProfileService, IDisposable
    {
        private DbContext _context;
        private UserManager<IdentityUser> _userManager;
        public ClientProfileService(DbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<OperationDetails> CreateOrUpdate(ClientProfileDTO clientProfileDto)
        {
            this.ThrowIfDisposed();

            var user = await _userManager.FindByEmailAsync(clientProfileDto.Email);
            if(user == null)
                return new OperationDetails(false, "Пользователь с таким e-mail не найден.", nameof(clientProfileDto.Email));

            var mapper = new MapperConfiguration(cfg=> cfg.CreateMap<ClientProfileDTO,ClientProfile>()).CreateMapper();
            var profileDbSet = _context.Set<ClientProfile>();
            OperationDetails opDetails;
            var clProfile = await profileDbSet.FindAsync(user.Id);
            
            if (clProfile != null)
            {
                clProfile = mapper.Map<ClientProfile>(clientProfileDto);
                profileDbSet.Update(clProfile);
                opDetails = new OperationDetails(true, "Профиль успешно изменен.", "");
            }
            else
            {
                clProfile = mapper.Map<ClientProfile>(clientProfileDto);
                profileDbSet.Add(clProfile);
                opDetails = new OperationDetails(true, "Профиль успешно создан.", "");
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                //TODO LOG this E
                return new OperationDetails(false, "Возникла ошибка при обновлении профиля.", "");
            }

            return opDetails;
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
                    _context?.Dispose();
                }

                // free unmanaged resources (unmanaged objects) and override a finalizer below.
                // set large fields to null.

                _disposed = true;
            }
        }

        // override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ClientProfileService() {
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
