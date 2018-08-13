using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Website.Data.EF.Models;
using Website.Service.Interfaces;

namespace Website.Web.Infrasctructure
{
    public class ApplicationSignInManager<TUser> : SignInManager<TUser> where TUser : class
    {

        private readonly UserManager<TUser> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IClientService _clientService;

        public ApplicationSignInManager
        (
            UserManager<TUser> userManager,
            IHttpContextAccessor contextAccessor,
            IUserClaimsPrincipalFactory<TUser> claimsFactory,
            IOptions<IdentityOptions> optionsAccessor,
            ILogger<SignInManager<TUser>> logger,
            IAuthenticationSchemeProvider schemes,
            IClientService clientService
        ) : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes)
        {
            if (userManager == null)
                throw new ArgumentNullException(nameof(userManager));

            if (clientService == null)
                throw new ArgumentNullException(nameof(clientService));

            if (contextAccessor == null)
                throw new ArgumentNullException(nameof(contextAccessor));

            _userManager = userManager;
            _contextAccessor = contextAccessor;
            _clientService = clientService;
        }
    }
}
