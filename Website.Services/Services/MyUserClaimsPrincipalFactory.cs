using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Website.Core.Interfaces.Services;
using Website.Core.Models.Domain;

namespace Website.Services.Services
{
    public class MyUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<User, Role>
    {
        private readonly IUserManager _userManager;
        public MyUserClaimsPrincipalFactory(
            IUserManager userManager,
            RoleManager<Role> roleManager,
            IOptions<IdentityOptions> optionsAccessor)
            : base(userManager as UserManager<User>, roleManager, optionsAccessor)
        {
            _userManager = userManager;
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(User user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            //
            //Ниже добавлять свои клеймы которые добавятся при логине в куки
            //
            identity.AddClaim(new Claim("Email", user.Email));
            identity.AddClaim(new Claim("FullName", user.FullName ?? ""));
            await _userManager.UpdateUserLastActivityDate(user.Id);

            return identity;
        }
    }
}
