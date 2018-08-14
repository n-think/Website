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
        private readonly IClientManager _clientManager;
        public MyUserClaimsPrincipalFactory(
            UserManager<UserDTO> userManager,
            RoleManager<RoleDTO> roleManager,
            IOptions<IdentityOptions> optionsAccessor,
            IClientManager clientManager)
            : base(userManager, roleManager, optionsAccessor)
        {
            _clientManager = clientManager;
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(UserDTO user)
        {
            var identity = await base.GenerateClaimsAsync(user);

            //
            //Ниже добавлять свои клеймы которые добавятся при логине в куки
            //

            identity.AddClaim(new Claim("Email", user.Email));

            //тут лениво загружается профиль, чтобы убрать ленивую загрузку надо сделать свой user manager который будет явно загружать профиль
            //или хранить данные профиля вместо с логинами
            identity.AddClaim(new Claim("FullName", user.ClientProfile?.FullName ?? ""));
            var asd = user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

            //fire and forget async log !not awaited
#pragma warning disable CS4014
            _clientManager.LogUserActivity(user.UserName);
#pragma warning restore CS4014

            return identity;
        }
    }
}
