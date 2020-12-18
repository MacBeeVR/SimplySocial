using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using SimplySocial.Server.Data.Identity;


namespace SimplySocial.Server.Configuration
{
    public static class ConfigExtensions
    {
        public static IServiceCollection ConfigureIdentity(this IServiceCollection services, IConfiguration config, IWebHostEnvironment env)
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
            
            if(env.IsProduction())
            {
                String certThumbPrint = config.GetValue<String>("SIGNING_CERTIFICATE");
                try
                {
                    var certBytes = File.ReadAllBytes($"/var/ssl/private/{certThumbPrint}.p12");
                    var signingCertificate = new X509Certificate2(certBytes);

                    services.AddIdentityServer()
                        .AddAspNetIdentity<User>()
                        .AddOperationalStore<IdentityContext>()
                        .AddIdentityResources()
                        .AddApiResources()
                        .AddClients()
                        .AddSigningCredential(signingCertificate);
                }
                catch(FileNotFoundException)
                {
                    throw new Exception($"Could not Locate Signing Certificate with Thumbprint: {certThumbPrint}");
                }
            }
            else
            {
                services.AddIdentityServer()
                    .AddApiAuthorization<User, IdentityContext>();
            }
            

            services.AddAuthentication()
                .AddIdentityServerJwt();

            return services;
        }

        public static IApplicationBuilder UseCustomStaticFiles(this IApplicationBuilder app)
        {
            app.UseStaticFiles();
            //app.UseStaticFiles(new StaticFileOptions()
            //{
            //    RequestPath     = new PathString("/css"),
            //    FileProvider    = new PhysicalFileProvider(Path.Combine())
            //     RedirectToAppendTrailingSlash = true
            //});

            return app;
        }
    }
}
