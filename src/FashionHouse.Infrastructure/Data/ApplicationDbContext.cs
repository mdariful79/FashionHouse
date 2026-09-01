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

    }
}
