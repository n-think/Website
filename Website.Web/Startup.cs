using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Website.Data.EF.Models;
using Website.Service.Interfaces;
using Website.Service.Services;
using Website.Web.Infrasctructure;
using Website.Web.Localization;
using Website.Web.Models;
using Website.Web.Resources;
using Website.Web.Services;

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


            services.AddDbContext<WebsiteContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))
                .UseLazyLoadingProxies());

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = false; // тк пользователь==емейл или будет две ошибки на валидации
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = false;
                options.Password.RequiredUniqueChars = 2;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Lockout.MaxFailedAccessAttempts = 5;
                //option.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            })

            .AddEntityFrameworkStores<WebsiteContext>()
                .AddDefaultTokenProviders()
                .AddErrorDescriber<RusIdentityErrorDescriberRes>();

            services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, MyUserClaimsPrincipalFactory>();

            services.Configure<SecurityStampValidatorOptions>(options =>
                options.ValidationInterval = TimeSpan.FromMinutes(30)); //FromSeconds(10));

            //services.ConfigureApplicationCookie(option =>
            //{
            //    //option.ExpireTimeSpan = TimeSpan.FromMinutes(30);
            //   // option.LoginPath = new PathString("/Login");
            //   // option.LogoutPath = new PathString("/Logout");
            //   // option.AccessDeniedPath = new PathString("/Login");
            //    //option.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            //});

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddScoped<DbContext, WebsiteContext>();
            services.AddScoped<IClientProfileService, ClientProfileService>();

            services.AddMvc()
                .AddDataAnnotationsLocalization(options =>
                    {
                        options.DataAnnotationLocalizerProvider = (type, factory) =>
                            factory.Create(typeof(SharedResource));
                    })
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IConfiguration configuration)
        {
            app.UseStatusCodePagesWithReExecute("/error/{0}");

            var errorMode = configuration.GetValue<string>("ErrorHandlingMode"); //читаем конфиг
            if (errorMode == "development" /*|| env.IsDevelopment()*/)
            {
                app.UseDeveloperExceptionPage();
            }

            else
            {
                app.UseExceptionHandler("/Error/exception");
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
