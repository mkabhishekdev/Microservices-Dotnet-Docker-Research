using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AbhiShop.Webmvc.Architecture;
using AbhiShop.Webmvc.Services;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace AbhiShop.Webmvc
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
            services.AddControllersWithViews();


            services.Configure<AppSettings>(Configuration);
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IHttpClient, CustomHttpClient>();
            services.AddTransient<IInventoryService, InventoryService>();

         
            var identityUrl = Configuration.GetValue<string>("IdentityUrl");
            var callBackUrl = Configuration.GetValue<string>("CallBackUrl");
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddOpenIdConnect(options => {
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                 
                options.Authority = identityUrl.ToString();
                options.SignedOutRedirectUri = callBackUrl.ToString();
                options.ClientId ="mvc";
                options.ClientSecret = "secret";
                options.ResponseType =  "code id_token";
                options.MapInboundClaims = false;
                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;
                options.ClaimActions.MapUniqueJsonKey("preferred_username", "preferred_username");
                options.RequireHttpsMetadata = false;
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("offline_access");
                options.TokenValidationParameters = new TokenValidationParameters()
                {

                    NameClaimType = "name",
                    RoleClaimType = "role"
                };



            });
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
                app.UseExceptionHandler("/Inventory/Error");
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Inventory}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                  name: "defaultError",
                  pattern: "{controller=Error}/{action=Error}");
            });
          
        }
    }
}
