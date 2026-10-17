using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TokenServiceAPI.Data;
using TokenServiceAPI.Models;
using TokenServiceAPI.Services;

namespace TokenServiceAPI
{
    public class Startup
    {
        private readonly IWebHostEnvironment _environment;

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            _environment = environment;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddControllersWithViews();

            // configure identity-server
            services.AddIdentityServer(options =>
                {
                    // Keep the existing developer signing setup for this learning project.
                    options.KeyManagement.Enabled = false;
                    if (_environment.IsDevelopment())
                    {
                        // Local HTTP services share a site; None without Secure is rejected by browsers.
                        options.Authentication.CookieSameSiteMode = SameSiteMode.Lax;
                        options.Authentication.CheckSessionCookieSameSiteMode = SameSiteMode.Lax;
                    }
                })
                .AddDeveloperSigningCredential(filename: "tempkey.net8.jwk")
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApiResources())
                .AddInMemoryApiScopes(Config.GetApiScopes())
                .AddInMemoryClients(Config.GetClients(Config.GetUrls(Configuration)))
                .AddAspNetIdentity<ApplicationUser>();

            if (_environment.IsDevelopment())
            {
                // Register after AddAspNetIdentity, which sets these cookies to SameSite=None.
                services.ConfigureApplicationCookie(options => options.Cookie.SameSite = SameSiteMode.Lax);
                services.ConfigureExternalCookie(options => options.Cookie.SameSite = SameSiteMode.Lax);
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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

            app.UseRouting();

            app.UseIdentityServer();  //middleware authentication
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
