using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Website.Service.DTO;

namespace Website.Service.Services
{
    public class SignInManager : SignInManager<UserDto>
    {
        public SignInManager(UserManager<UserDto> userManager,
            IHttpContextAccessor contextAccessor,
            IUserClaimsPrincipalFactory<UserDto> claimsFactory,
            IOptions<IdentityOptions> optionsAccessor,
            ILogger<SignInManager<UserDto>> logger,
            IAuthenticationSchemeProvider schemeProvider)
            : base(userManager, contextAccessor, claimsFactory,
                optionsAccessor, logger, schemeProvider)
        {
        }
    }
}
