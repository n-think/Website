using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Website.Service.DTO;

namespace Website.Service.Services
{
    public class SignInManager : SignInManager<UserDTO>
    {
        public SignInManager(UserManager<UserDTO> userManager,
            IHttpContextAccessor contextAccessor,
            IUserClaimsPrincipalFactory<UserDTO> claimsFactory,
            IOptions<IdentityOptions> optionsAccessor,
            ILogger<SignInManager<UserDTO>> logger,
            IAuthenticationSchemeProvider schemeProvider)
            : base(userManager, contextAccessor, claimsFactory,
                optionsAccessor, logger, schemeProvider)
        {
        }
    }
}
