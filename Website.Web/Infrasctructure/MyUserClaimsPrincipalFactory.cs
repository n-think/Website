using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Website.Data.EF.Models;

namespace Website.Web.Infrasctructure
{
    public class MyUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
    {
        public MyUserClaimsPrincipalFactory(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, roleManager, optionsAccessor)
        {
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


            return identity;
        }
    }
}
