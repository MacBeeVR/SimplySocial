using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplySocial.Server.Data.UserData
{
    public class UserDataContext : DbContext
    {
        private readonly IWebHostEnvironment _hostEnvironment;

        #region Constructors
        public UserDataContext() 
        { }

        public UserDataContext(DbContextOptions<UserDataContext> options, IWebHostEnvironment hostEnvironment) 
            : base(options) 
        {
            _hostEnvironment = hostEnvironment;
        }
        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_hostEnvironment.IsDevelopment())
            {
                optionsBuilder.EnableDetailedErrors();
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            
        }
    }
}
