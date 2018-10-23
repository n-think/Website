using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
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
using Website.Web.Resources;
using Website.Service.Stores;
using Website.Web.Infrastructure;
using Website.Web.Infrastructure.Localization;
using Website.Web.Infrastructure.Mapper;


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

            services.AddDbContext<WebsiteDbContext>(/*ServiceLifetime.Transient*/);
            services.AddAutoMapper(opt =>
            {
                opt.AddProfile<ServiceProfile>();
                opt.AddProfile<WebsiteProfile>();
            });

            services.AddIdentity<UserDto, RoleDto>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = false;
                options.Password.RequiredUniqueChars = 2;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Lockout.MaxFailedAccessAttempts = 10;
                //option.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            }).AddUserManager<UserManager>()
                .AddRoleManager<RoleManager>()
                .AddSignInManager<SignInManager>()
                .AddDefaultTokenProviders()
                .AddErrorDescriber<RusIdentityErrorDescriberRes>();
            AddCustomInterfaces(services);

            services.AddScoped<IUserClaimsPrincipalFactory<UserDto>, MyUserClaimsPrincipalFactory>();

            services.Configure<SecurityStampValidatorOptions>(options =>
            {
                //options.ValidationInterval = TimeSpan.FromSeconds(2);
                options.ValidationInterval = TimeSpan.FromMinutes(10);
            });

            services.ConfigureApplicationCookie(option =>
            {
                //option.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                // option.LoginPath = new PathString("/Login");
                // option.LogoutPath = new PathString("/Logout");
                option.AccessDeniedPath = new PathString("/error/403");
                //option.SlidingExpiration = true;
                //option.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });

            services.AddScoped<DbContext, WebsiteDbContext>();

            services.AddMvc(config =>
                {
                    config.ModelBinderProviders.Insert(0, new InvariantDecimalModelBinderProvider());
                })
                .AddDataAnnotationsLocalization(options =>
                    {
                        options.DataAnnotationLocalizerProvider = (type, factory) =>
                            factory.Create(typeof(SharedResource));
                    })
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            AddPolicies(services);
        }

        private void AddCustomInterfaces(IServiceCollection services)
        {
            services.AddTransient<IRoleStore<RoleDto>, CustomRoleStore>();
            services.AddTransient<IUserStore<UserDto>, CustomUserStore>();
            services.AddTransient<IShopStore<ProductDto, ProductImageDto, OrderDto>, ShopStore>();

            services.AddScoped<IUserManager, UserManager>();
            services.AddScoped<IShopManager, ShopManager>();

            services.AddTransient<IEmailSender, EmailSender>();
        }

        private void AddPolicies(IServiceCollection services)
        {
            services.AddAuthorization(options =>
                {
                    //global admin policy
                    options.AddPolicy("Administrators",
                        policy => policy.RequireAssertion(context =>
                            context.User.IsInRole("admin") || context.User.IsInRole("admin_generated")));

                    //user manage policies
                    options.AddPolicy("ViewUsers",
                        policy => policy.RequireAssertion(context => context.User.HasClaim(c =>
                                c.Type == "ViewUsers" || c.Type == "EditUsers" || c.Type == "DeleteUsers")));
                    options.AddPolicy("EditUsers",
                        policy => policy.RequireAssertion(context => context.User.HasClaim(c =>
                            c.Type == "EditUsers" || c.Type == "DeleteUsers")));
                    options.AddPolicy("DeleteUsers",
                        policy => policy.RequireClaim("DeleteUsers"));

                    //item manage policies
                    options.AddPolicy("ViewItems",
                        policy => policy.RequireAssertion(context => context.User.HasClaim(c =>
                            c.Type == "ViewItems" || c.Type == "EditItems" || c.Type == "DeleteItems" || c.Type == "CreateItems")));

                    options.AddPolicy("EditItems",
                        policy => policy.RequireAssertion(context => context.User.HasClaim(c =>
                            c.Type == "EditItems" || c.Type == "DeleteItems" || c.Type == "CreateItems")));

                    options.AddPolicy("CreateItems",
                        policy => policy.RequireAssertion(context => context.User.HasClaim(c =>
                            c.Type == "DeleteItems" || c.Type == "CreateItems")));

                    options.AddPolicy("DeleteItems",
                        policy => policy.RequireClaim("DeleteItems"));

                    //orders manage policies
                    options.AddPolicy("ViewOrders",
                        policy => policy.RequireAssertion(context => context.User.HasClaim(c =>
                            c.Type == "ViewOrders" || c.Type == "EditOrders" || c.Type == "DeleteOrders")));
                    options.AddPolicy("EditOrders",
                        policy => policy.RequireAssertion(context => context.User.HasClaim(c =>
                            c.Type == "EditOrders" || c.Type == "DeleteOrders")));
                    options.AddPolicy("DeleteUOrders",
                        policy => policy.RequireClaim("DeleteOrders"));
                }
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IConfiguration configuration)
        {
            var cultureInfo = new CultureInfo("ru-RU");// { NumberFormat = { CurrencySymbol = "₽" } };
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            app.UseStatusCodePagesWithReExecute("/error/{0}");// ломает коды

            var errorMode = configuration.GetValue<bool>("DevelopmentErrorHandlingMode"); //читаем конфиг
            if (errorMode || env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            else
            {
                //TODO тут мб логировать
                app.UseExceptionHandler("/error/500");
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
