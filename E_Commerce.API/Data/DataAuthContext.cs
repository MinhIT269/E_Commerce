using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.API.Data
{
    public class DataAuthContext : IdentityDbContext
    {
        public DataAuthContext(DbContextOptions<DataAuthContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var roles = new List<IdentityRole>
            {
                new IdentityRole{Name = "Admin", NormalizedName = "ADMIN", ConcurrencyStamp = Guid.NewGuid().ToString()},
                new IdentityRole{Name = "User", NormalizedName = "USER", ConcurrencyStamp = Guid.NewGuid().ToString()},
            };

            builder.Entity<IdentityRole>().HasData(roles);
        }
    }
}
