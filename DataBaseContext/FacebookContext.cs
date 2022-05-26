using Facebook.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Facebook.DataBaseContext
{
    public class FacebookContext: IdentityDbContext<CustomUserFields>
    {
        private readonly DbContextOptions _options;

        public FacebookContext(DbContextOptions<FacebookContext> options) : base(options)
        {
            _options = options;
        }

       

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }
    }
}
