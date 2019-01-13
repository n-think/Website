using System;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Website.Services.Services;
using Website.Web.Resources;
using Website.Web.Infrastructure;
using Website.Web.Infrastructure.Localization;
using Website.Web.Infrastructure.Mapper;
using Microsoft.AspNetCore.DataProtection;
using Website.Core.Enums;
using Website.Core.Infrastructure;
using Website.Core.Interfaces.Repositories;
using Website.Core.Interfaces.Services;
using Website.Core.Models.Domain;
using Website.Data.EF;
using Website.Data.EF.Repositories;
using Website.Services.Infrastructure;
using Website.Services.Infrastructure.Validators;

namespace Website.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<WebsiteDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped<DbContext, WebsiteDbContext>();

            services.AddAutoMapper(opt => { opt.AddProfile<WebsiteProfile>(); });

            services.AddOptions();
            services.Configure<ShopManagerOptions>(options =>
            {
                options.Image.SaveFormat = ImageFormat.Jpeg;
                options.Image.EncoderQuality = 80L;
                options.Image.MaxWidth = 1000;
                options.Image.MaxWidth = 1000;
                options.Image.MaxThumbWidth = 150;
                options.Image.MaxThumbHeight = 150;
            });

            services.AddIdentity<User, Role>(options =>
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
                })
                .AddUserManager<UserManager>()
                .AddRoleManager<RoleManager>()
                .AddSignInManager<SignInManager>()
                .AddDefaultTokenProviders()
                .AddErrorDescriber<RusIdentityErrorDescriberRes>()
                .AddEntityFrameworkStores<WebsiteDbContext>();

            AddMyServices(services);

            services.AddScoped<IUserClaimsPrincipalFactory<User>, MyUserClaimsPrincipalFactory>();

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
                option.Events.OnRedirectToLogin = (context) =>
                {
                    if (context.Request.Path.StartsWithSegments("/api/admin") && context.Response.StatusCode == 200)
                    {
                        context.Response.StatusCode = 404;
                    }
                    else
                    {
                        context.Response.Redirect(context.RedirectUri);
                    }

                    return Task.CompletedTask;
                };
                option.Events.OnRedirectToAccessDenied = (context) =>
                {
                    if (context.Request.Path.StartsWithSegments("/api/admin") && context.Response.StatusCode == 200)
                    {
                        context.Response.StatusCode = 403;
                    }
                    else
                    {
                        context.Response.Redirect(context.RedirectUri);
                    }

                    return Task.CompletedTask;
                };
            });

            services.AddMvc(config =>
                {
                    config.ModelBinderProviders.Insert(0, new InvariantDecimalModelBinderProvider());
                })
                .AddDataAnnotationsLocalization(options =>
                {
                    options.DataAnnotationLocalizerProvider = (type, factory) =>
                        factory.Create(typeof(SharedResource));
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            AddPolicies(services);
            services.AddDataProtection()
                // This helps surviving a restart: a same app will find back its keys.
                .PersistKeysToFileSystem(new DirectoryInfo(Environment.ContentRootPath + "\\keys"))
                // This helps surviving a site update: each app has its own store, building the site creates a new app
                .SetApplicationName("MyWebsite");
            //.ProtectKeysWithDpapi();
            //.SetDefaultKeyLifetime(TimeSpan.FromDays(90)); //default 90 days
        }

        private void AddMyServices(IServiceCollection services)
        {
            services.AddTransient<
                IShopRepository<Product, Image, ImageBinData, Category,  ProductToCategory, DescriptionGroup,
                    DescriptionGroupItem, Description, Order>, ShopRepository>();
            services.AddTransient<IUserStore<User>, UserRepository>();

            services.AddScoped<OperationErrorDescriber>();
            services.AddScoped<IUserManager, UserManager>();
            services.AddScoped<IShopManager, ShopManager>();

            services.AddScoped<IShopValidator<Product>, ProductValidator>();
            services.AddScoped<IShopValidator<Image>, ImageValidator>();
            services.AddScoped<IShopValidator<Category>, CategoryValidator>();
            services.AddScoped<IShopValidator<DescriptionGroup>, DescriptionGroupValidator>();
            services.AddScoped<IShopValidator<Description>, DescriptionValidator>();
            services.AddScoped<IShopValidator<Order>, OrderValidator>();
            services.AddScoped<IShopValidator<Order>, OrderValidator>();

            services.AddScoped<IShopImageTransformer<Image>, ShopImageTransformer>();

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
                            c.Type == "ViewItems" || c.Type == "EditItems" || c.Type == "DeleteItems")));

                    options.AddPolicy("EditItems",
                        policy => policy.RequireAssertion(context => context.User.HasClaim(c =>
                            c.Type == "EditItems" || c.Type == "DeleteItems")));

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
            var cultureInfo = new CultureInfo("ru-RU") {NumberFormat = {CurrencySymbol = "₽"}};
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            app.UseStatusCodePagesWithReExecute("/error/{0}");

            var errorMode = configuration.GetValue<bool>("DevelopmentErrorHandlingMode");
            if (errorMode || env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            else
            {
                app.UseExceptionHandler("/error/500");
                app.UseHsts();
            }

            app.UseStaticFiles();
            //app.UseHttpsRedirection(); //no need behind iis
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}"); //{controller=Home}/{action=Index}/{id?}
            });
        }
    }
}