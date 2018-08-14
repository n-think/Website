using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Website.Service.DTO;

namespace Website.Service.Services
{
    public class UserManager : UserManager<UserDTO>
    {
        public UserManager(IUserStore<UserDTO> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<UserDTO> passwordHasher,
            IEnumerable<IUserValidator<UserDTO>> userValidators,
            IEnumerable<IPasswordValidator<UserDTO>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<UserManager<UserDTO>> logger)
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
        }
    }
}
