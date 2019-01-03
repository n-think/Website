using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Website.Core.DTO;

namespace Website.Services.Services
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
