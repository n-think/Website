using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Website.Data.EF.Models;
using Website.Service.Interfaces;

namespace Website.Web.Infrasctructure
{
    public class MyUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
    {
        private IClientService _clientService;
        public MyUserClaimsPrincipalFactory(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<IdentityOptions> optionsAccessor,
            IClientService clientService)
            : base(userManager, roleManager, optionsAccessor)
        {
            _clientService = clientService;
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);

            //
            //Ниже добавлять свои клеймы которые добавятся при логине в куки
            //

            identity.AddClaim(new Claim("Email", user.Email));

            //тут лениво загружается профиль, чтобы убрать ленивую загрузку надо сделать свой user manager который будет явно загружать профиль
            //или хранить данные профиля вместо с логинами
            identity.AddClaim(new Claim("FullName", user.ClientProfile?.FullName ?? ""));
            var asd = user.Claims.FirstOrDefault(x => x.ClaimType == ClaimTypes.NameIdentifier);

            //fire and forget async log !not awaited
#pragma warning disable CS4014
            _clientService.LogUserActivity(user.UserName);
#pragma warning restore CS4014

            return identity;
        }
    }
}
