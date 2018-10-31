using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Website.Service.DTO;

namespace Website.Service.Services
{
    public class RoleManager : RoleManager<RoleDto>
    {
        public RoleManager(IRoleStore<RoleDto> store,
            IEnumerable<IRoleValidator<RoleDto>> roleValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            ILogger<RoleManager<RoleDto>> logger)
            : base(store, roleValidators, keyNormalizer, errors, logger)
        {
        }
    }
}
