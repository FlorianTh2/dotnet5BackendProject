using dotnet5BackendProject.Data;
using dotnet5BackendProject.Domain;
using dotnet5BackendProject.Extensions;
using dotnet5BackendProject.Installers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace dotnet5BackendProject
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        // This method gets called by the runtime. Use this method to add services to the IoC-container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.InstallDb(Configuration);

            services.InstallMvc(Configuration);
            
            services.InstallAutomapper();

            services.InstallCacheRedis(Configuration);

            services.InstallSwagger();

            services.InstallHealthCheck();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // app.UseExceptionHandler();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "dotnet5BackendProject v1"));
                
                // app.Use(async (context, next) =>
            // {
            //     logger.LogInformation("Middleware (MW) 1: Incoming Request");
            //     await next();
            //     logger.LogInformation("MW2: Outgoing Response");
            // });

            app.UseCustomHealthChecks();
            
            app.UseHttpsRedirection();
            
            app.UseRouting();


            // https://www.codeproject.com/Articles/5160941/ASP-NET-CORE-Token-Authentication-and-Authorizatio
            
            // introduction: https://docs.microsoft.com/de-de/aspnet/core/security/authentication/?view=aspnetcore-5.0
            // -> this authentication middleware uses internally the service: IAuthenticationService
            // -> the authentication-service uses registered authentication-handlers
            // -> The registered authentication handlers and their configuration options are called "schemes".
            // -> in this project the jwt-authentication-scheme is registered in the MvcService (with services.AddAuthentication(/* */)
            // -> this authentication-handler/-scheme contains authentication-related actions
            // -> actions e.g.:
            //      - decrypt the ascii-"encoding" (!=Verschluessellung)
            //      - validate the content of the jwt with the given secret
            //      - Setting the User object in HTTP Request Context
            //      - set the IsAuthenticated 
            app.UseAuthentication();

            app.UseAuthorization();
            
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}