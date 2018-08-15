using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Website.Data.EF.Models;
using Website.Service.DTO;
using Website.Service.Interfaces;
using Website.Service.Mapper;
using Website.Service.Services;
using Website.Web.Localization;
using Website.Web.Resources;
using Website.Service.IdentityStores;


namespace Website.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<WebsiteDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))
                .UseLazyLoadingProxies());

            services.AddDbContext<WebsiteDbContext>();
            services.AddAutoMapper(opt => opt.AddProfile<MapperProfile>());

            services.AddIdentity<UserDTO, RoleDTO>(options =>
            {
                options.User.RequireUniqueEmail = false; // false тк пользователь==емейл или будет две ошибки на валидации //TODO если можно менять имя надо true
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = false;
                options.Password.RequiredUniqueChars = 2;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Lockout.MaxFailedAccessAttempts = 10;
                //option.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            })
                //.AddEntityFrameworkStores<WebsiteDbContext>()
                .AddUserManager<UserManager>()
                .AddRoleManager<RoleManager>()
                .AddSignInManager<SignInManager>()
                .AddDefaultTokenProviders()
                .AddErrorDescriber<RusIdentityErrorDescriberRes>();
            FixInterfaces(services);

            //custom sign in manager. пока не нужен
            //services.AddScoped<SignInManager<UserDTO>, ApplicationSignInManager<UserDTO>>();

            services.AddScoped<IUserClaimsPrincipalFactory<UserDTO>, MyUserClaimsPrincipalFactory>();

            services.Configure<SecurityStampValidatorOptions>(options =>
            {
                //options.ValidationInterval = TimeSpan.FromSeconds(1);
                options.ValidationInterval = TimeSpan.FromMinutes(10);
            });


            services.ConfigureApplicationCookie(option =>
            {
                //option.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                // option.LoginPath = new PathString("/Login");
                // option.LogoutPath = new PathString("/Logout");
                option.AccessDeniedPath = new PathString("/error/403");
                //option.SlidingExpiration = 
                //option.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });

            services.AddTransient<IEmailSender, EmailSender>();

            services.AddScoped<DbContext, WebsiteDbContext>();
            services.AddScoped<IUserManager, UserManager>();

            services.AddMvc()
                .AddDataAnnotationsLocalization(options =>
                    {
                        options.DataAnnotationLocalizerProvider = (type, factory) =>
                            factory.Create(typeof(SharedResource));
                    })
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        private void FixInterfaces(IServiceCollection services)
        {
            services.AddTransient<IRoleStore<RoleDTO>, CustomRoleStore>();
            services.AddTransient<IUserStore<UserDTO>, CustomUserStore>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IConfiguration configuration)
        {
            var cultureInfo = new CultureInfo("ru-RU");// { NumberFormat = { CurrencySymbol = "₽" } };
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            app.UseStatusCodePagesWithReExecute("/error/{0}");

            var errorMode = configuration.GetValue<string>("ErrorHandlingMode"); //читаем конфиг
            if (errorMode == "development" /*|| env.IsDevelopment()*/)
            {
                app.UseDeveloperExceptionPage();
            }

            else
            {
                //TODO тут надо логировать както или мб уже логируется
                app.UseExceptionHandler("/error/exception");
                app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
