using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryManagementAPI.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace InventoryManagementAPI
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
            services.Configure<InventorySettings>(Configuration);

            var server = Configuration["DatabaseServer"];
            var database = Configuration["DatabaseName"];
            var user = Configuration["DatabaseUser"];
            var password = Configuration["DatabaseUserPassword"];
            // Keep Docker's database settings, and use appsettings for local runs.
            var connectionString = String.IsNullOrWhiteSpace(server)
                ? Configuration["ConnectionString"]
                : String.Format("Server={0};Database={1};User={2};Password={3};",server,database,user,password);
            var connection = new SqlConnectionStringBuilder(connectionString);
            if (Configuration.GetValue<bool>("DatabaseTrustServerCertificate"))
            {
                connection.TrustServerCertificate = true;
            }

          //  services.AddDbContext<InventoryContext>(options=>options.UseSqlServer(Configuration["ConnectionString"]));
            services.AddDbContext<InventoryContext>(options=>options.UseSqlServer(connection.ConnectionString));

            services.AddControllersWithViews().AddNewtonsoftJson();

            //swagger code generation code
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1",new OpenApiInfo
                {
                    Title = "AbhiShop - Inventory Catalog API",
                    Version = "v1",
                    Description = "Inventory Microservice"
                });
            });
            services.AddSwaggerGenNewtonsoftSupport();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger()
             .UseSwaggerUI(c =>
             {
                 c.SwaggerEndpoint($"/swagger/v1/swagger.json","InventoryAPI V1");
             });
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        } 
    }
}

