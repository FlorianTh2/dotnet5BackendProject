﻿using dotnet5BackendProject.Data;
using dotnet5BackendProject.Domain;
using dotnet5BackendProject.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace dotnet5BackendProject.Installers
{
    public static class DbInstaller
    {
        public static void InstallDb(this IServiceCollection services, IConfiguration Configuration)
        {
            
            services.AddHttpContextAccessor();

            services.AddSingleton<ICurrentUserService, CurrentUserService>();
            
            services.AddTransient<IDateTime, DateTimeService>();
            
            
            services.AddDbContext<DataContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("dotnet5BackendProjectContextPostgre")));

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddRoles<IdentityRole>()
                // to make entityFramework function with identity
                .AddEntityFrameworkStores<DataContext>();
            

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 10;
                options.Password.RequiredUniqueChars = 3;
            });

            // changed to scoped because of tracking?!
            // services.AddSingleton<IProjectService, ProjectService>();
            
            services.AddScoped<ITagService, TagService>();
            
            services.AddScoped<IProjectService, ProjectService>();
            
            services.AddScoped<IDocumentService, DocumentService>();
        }
    }
}