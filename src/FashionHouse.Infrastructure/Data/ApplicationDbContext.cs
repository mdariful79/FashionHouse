using FashionHouse.Domain.Entities;
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
        public DbSet<Category> Categories { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Product>()
                .HasOne(x => x.Category)
                .WithMany()
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Product>()
                .HasOne(x => x.SubCategory)
                .WithMany()
                .HasForeignKey(x => x.SubCategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ProductImage>()
                .HasOne(x => x.Product)
                .WithMany(x => x.ProductImages)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Inventory>()
                .HasOne(x => x.Product)
                .WithOne(x => x.Inventory)
                .HasForeignKey<Inventory>(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Inventory>()
                .HasIndex(x => x.ProductId)
                .IsUnique();
            builder.Entity<Customer>()
                .HasIndex(x => x.Email)
                .IsUnique();

            builder.Entity<Address>()
                .HasOne(x => x.Customer)
                .WithMany(x => x.Addresses)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Cart>()
                .HasOne(x => x.Customer)
                .WithOne(x => x.Cart)
                .HasForeignKey<Cart>(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Cart>()
                .HasIndex(x => x.CustomerId)
                .IsUnique();

         
            builder.Entity<Cart>()
                .Property(x => x.Id)
                .ValueGeneratedNever();

            builder.Entity<CartItem>()
                .Property(x => x.Id)
                .ValueGeneratedNever();

            builder.Entity<CartItem>()
                .HasOne(x => x.Cart)
                .WithMany(x => x.CartItems)
                .HasForeignKey(x => x.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CartItem>()
                .HasOne(x => x.Product)
                .WithMany(x => x.CartItems)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Order>()
                .HasOne(x => x.Customer)
                .WithMany(x => x.Orders)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Order>()
                .HasIndex(x => x.OrderNumber)
                .IsUnique();

            builder.Entity<Order>()
                .Property(x => x.SubTotal).HasColumnType("decimal(18,2)");
            builder.Entity<Order>()
                .Property(x => x.ShippingFee).HasColumnType("decimal(18,2)");
            builder.Entity<Order>()
                .Property(x => x.Discount).HasColumnType("decimal(18,2)");
            builder.Entity<Order>()
                .Property(x => x.GrandTotal).HasColumnType("decimal(18,2)");

            builder.Entity<OrderItem>()
                .HasOne(x => x.Order)
                .WithMany(x => x.OrderItems)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<OrderItem>()
                .HasOne(x => x.Product)
                .WithMany(x => x.OrderItems)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict); // keep order history even if a product is later deleted

            builder.Entity<OrderItem>()
                .Property(x => x.UnitPrice).HasColumnType("decimal(18,2)");
            builder.Entity<OrderItem>()
                .Property(x => x.LineTotal).HasColumnType("decimal(18,2)");

            builder.Entity<ApplicationRole>().HasData(Seeds.RoleSeeds.GetRoles());
        }
    }
}