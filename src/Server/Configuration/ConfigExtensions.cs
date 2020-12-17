using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using SimplySocial.Server.Data.Identity;

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

            var env = config.GetValue<String>("ASPNETCORE_ENVIRONMENT");
            if(!String.IsNullOrWhiteSpace(env) && env.ToLowerInvariant() == "production")
            {
                var certPath    = $"/var/ssl/private/{config.GetValue<String>("WEBSITE_LOAD_CERTIFICATES")}.p12";
                var certBytes   = File.ReadAllBytes(certPath);
                var certificate = new X509Certificate2(certBytes);

                services.AddIdentityServer()
                    .AddSigningCredential(certificate)
                    .AddApiAuthorization<User, IdentityContext>();
            }
            else
            {
                services.AddIdentityServer()
                    .AddDeveloperSigningCredential()
                    .AddApiAuthorization<User, IdentityContext>();
            }
            

            services.AddAuthentication().AddIdentityServerJwt();

            return services;
        }
    }
}
