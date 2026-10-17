using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TokenServiceAPI.Models;

namespace TokenServiceAPI.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Preserve the schema created by the original CreateIdentitySchema migration.
            builder.Entity<IdentityRole>().HasIndex(role => role.NormalizedName).IsUnique(false);
            builder.Entity<ApplicationUser>().HasIndex(user => user.NormalizedUserName).HasFilter(null);
            builder.Entity<IdentityUserRole<string>>().HasIndex(role => role.UserId);

            var userTokens = builder.Entity<IdentityUserToken<string>>().Metadata;
            foreach (var foreignKey in userTokens.GetForeignKeys().ToList())
            {
                userTokens.RemoveForeignKey(foreignKey);
            }
        }
    }
}
