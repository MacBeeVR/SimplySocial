using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;

using IdentityServer4.EntityFramework.Options;
using IdentityServer4.EntityFramework.Extensions;

namespace SimplySocial.Server.Data.Identity
{
    public class IdentityContext : ApiAuthorizationDbContext<User>
    {
        private readonly IWebHostEnvironment _hostEnvironment;

        public IdentityContext(
            IWebHostEnvironment hostEnvironment,
            DbContextOptions<IdentityContext> options,
            IOptions<OperationalStoreOptions> operationalStoreOptions)
                : base(options, operationalStoreOptions)
        {
            _hostEnvironment = hostEnvironment;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_hostEnvironment.IsDevelopment())
            {
                optionsBuilder.EnableDetailedErrors();
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configuring custom Identity user
            builder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.Property(e => e.FirstName)
                    .HasMaxLength(50)
                    .HasColumnName("FirstName");

                entity.Property(e => e.LastName)
                    .HasMaxLength(50)
                    .HasColumnName("LastName");

                entity.Ignore(e => e.FullName);
            });

            // Changing default Identity table names
            builder.Entity<IdentityRole>             (entity => entity.ToTable("Roles"));
            builder.Entity<IdentityUserRole<String>> (entity => entity.ToTable("UserRoles"));
            builder.Entity<IdentityUserClaim<String>>(entity => entity.ToTable("UserClaims"));
            builder.Entity<IdentityUserLogin<String>>(entity => entity.ToTable("UserLogins"));
            builder.Entity<IdentityRoleClaim<String>>(entity => entity.ToTable("RoleClaims"));
            builder.Entity<IdentityUserToken<String>>(entity => entity.ToTable("UserTokens"));
        }
    }
}
