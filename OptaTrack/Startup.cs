using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using AccountService.Repositories.Contracts;
using AccountService.Repositories.Implementations;
using AccountService.Services.Contracts;
using AccountService.Services.Implementations;

namespace OptaTrack
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
            services.Configure<CookiePolicyOptions>(cookieOptions =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                cookieOptions.CheckConsentNeeded = context => true;
                cookieOptions.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddAuthentication("OptaTrack")
                    .AddCookie("OptaTrack", authOptions =>
                    {
                        authOptions.AccessDeniedPath = new PathString("/Error/404");
                        authOptions.LoginPath = new PathString("/Account/Login");
                    });

            services.AddAuthorization(auth =>
            {
                auth.AddSecurity();
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            RegisterDependencies(services);
            
            Action<OptaTrackOptions> options = (opt =>
            {
                opt.ConnectionString = Configuration["ConnectionStrings:Connection"];
            });
            services.Configure(options);
            services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<OptaTrackOptions>>().Value);

            Action<AccountService.AccountServiceOptions> acctOptions = (opt =>
            {
                opt.AppDBConnection = Configuration["ConnectionStrings:Connection"];
            });
            services.Configure(acctOptions);
            services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<AccountService.AccountServiceOptions>>().Value);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private void RegisterDependencies(IServiceCollection services)
        {
            //// Add application services.
            //services.AddTransient<IEmailSender, AuthMessageSender>();
            //services.AddTransient<ISmsSender, AuthMessageSender>();
            
            // services
            services.AddSingleton<IUserDataService, UserDataService>();
            
            // repositories
            services.AddSingleton<IUserRepository, UserRepository>();
            services.AddSingleton<IUserRoleRepository, UserRoleRepository>();

        }
    }
}
