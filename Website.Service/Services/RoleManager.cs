using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Website.Service.DTO;

namespace Website.Service.Services
{
    public class RoleManager : RoleManager<RoleDTO>
    {
        public RoleManager(IRoleStore<RoleDTO> store,
            IEnumerable<IRoleValidator<RoleDTO>> roleValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            ILogger<RoleManager<RoleDTO>> logger)
            : base(store, roleValidators, keyNormalizer, errors, logger)
        {
        }
    }
}
