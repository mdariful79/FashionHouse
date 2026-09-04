using FashionHouse.Domain.Entites;
using FashionHouse.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FashionHouse.Infrastructure.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser,
      ApplicationRole,
      Guid,
      ApplicationUserClaim,
      ApplicationUserRole,
      ApplicationUserLogin,
      ApplicationRoleClaim,
      ApplicationUserToken>(options)
    {
        public DbSet<Product> Products { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ApplicationRole>().HasData(Seeds.RoleSeeds.GetRoles());
        }
    }
}
