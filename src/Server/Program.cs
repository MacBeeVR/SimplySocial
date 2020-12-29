using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SimplySocial.Server.Core.Settings;
using SimplySocial.Server.Data.Identity;

namespace SimplySocial.Server
{
    public class Program
    {
        public static async Task Main(String[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                // Performing Database Migrations and Admin User / Role Data Seed in Development Environment
                var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
                if (env.IsDevelopment())
                {
                    var identityContext = scope.ServiceProvider.GetRequiredService<IdentityContext>();
                    await identityContext.Database.MigrateAsync();

                    var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
                    var adminSeed = config.GetSection("AdminSeed");
                    if(adminSeed != null)
                    {
                        var seedSettings = new AdminSeedSettings();
                        config.Bind("AdminSeed", seedSettings);

                        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                        if (!await roleManager.RoleExistsAsync("User"))
                            await roleManager.CreateAsync(new IdentityRole("User"));

                        if (!await roleManager.RoleExistsAsync("Admin"))
                            await roleManager.CreateAsync(new IdentityRole("Admin"));

                        if(await userManager.FindByEmailAsync(seedSettings.Email) == null)
                        {
                            var admin = new User
                            {
                                EmailConfirmed      = true,
                                LockoutEnabled      = false,
                                Email               = seedSettings.Email,
                                UserName            = seedSettings.UserName,
                                SecurityStamp       = Guid.NewGuid().ToString(),
                                NormalizedEmail     = seedSettings.Email.ToUpperInvariant(),
                                NormalizedUserName  = seedSettings.UserName.ToUpperInvariant()
                            };

                            var pwdHasher = new PasswordHasher<User>();
                            var hashedPwd = pwdHasher.HashPassword(admin, seedSettings.Password);

                            admin.PasswordHash = hashedPwd;

                            var userStore = new UserStore<User>(identityContext);
                            await userStore.CreateAsync(admin);
                            await userStore.AddToRoleAsync(admin, "User");
                            await userStore.AddToRoleAsync(admin, "Admin");

                            await identityContext.SaveChangesAsync();
                        }
                    }
                }
            }

            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(String[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });


    }
}
