using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SimplySocial.Server.Data.Identity;

namespace SimplySocial.Server
{
    public class Program
    {
        public static async Task Main(String[] args)
        {
            var host = CreateHostBuilder(args).Build();

#if DEBUG
            using (var scope = host.Services.CreateScope())
            {
                var identityContext = scope.ServiceProvider.GetRequiredService<IdentityContext>();
                await identityContext.Database.MigrateAsync();
            }
#endif

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
