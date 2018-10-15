using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Website.Service.DTO;
using Website.Service.Interfaces;

namespace Website.Service.Services
{
    public class MyUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<UserDTO, RoleDTO>
    {
        private readonly IUserManager _userManager;
        public MyUserClaimsPrincipalFactory(
            IUserManager userManager,
            RoleManager<RoleDTO> roleManager,
            IOptions<IdentityOptions> optionsAccessor)
            : base(userManager as UserManager<UserDTO>, roleManager, optionsAccessor)
        {
            _userManager = userManager;
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(UserDTO user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            //
            //Ниже добавлять свои клеймы которые добавятся при логине в куки
            //
            identity.AddClaim(new Claim("Email", user.Email));
            identity.AddClaim(new Claim("FullName", user?.UserProfile?.FullName ?? ""));
            await _userManager.LogUserActivity(user.UserName);

            return identity;
        }
    }
}
