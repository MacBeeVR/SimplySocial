using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using SimplySocial.Server.Data.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;

namespace SimplySocial.Server.Configuration
{
    public static class ConfigExtensions
    {
        public static IServiceCollection ConfigureIdentity(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<IdentityContext>(options =>
                options.UseSqlServer(config.GetConnectionString("SimplySocialDB")));

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddIdentity<User, IdentityRole>()
               .AddEntityFrameworkStores<IdentityContext>()
               .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers      = true;
                options.User.RequireUniqueEmail         = true;
                options.SignIn.RequireConfirmedEmail    = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Lockout.DefaultLockoutTimeSpan  = TimeSpan.FromMinutes(5);
            });

            services.AddIdentityServer().AddApiAuthorization<User, IdentityContext>();
            services.AddAuthentication().AddIdentityServerJwt();

            return services;
        }
    }
}
