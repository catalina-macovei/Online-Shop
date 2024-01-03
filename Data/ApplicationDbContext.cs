using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Models;

namespace OnlineShop.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Comment> Comments { get; set; }

        public DbSet<Cart> Carts { get; set; }

        public DbSet<Order> Orders { get; set; }
        public DbSet<Ordered_Product> Ordered_Products { get; set; }

        protected override void OnModelCreating(ModelBuilder
        modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // definire primary key compus
            modelBuilder.Entity<Cart>()
            .HasKey(c => new
            {
                c.Id,
                c.UserId,
                c.ProductId
            });

            // definire relatii cu modelele ApplicationUser si Product (FK)
            modelBuilder.Entity<Cart>()
            .HasOne(c => c.Product)
            .WithMany(c => c.Carts)
            .HasForeignKey(c => c.ProductId);
            modelBuilder.Entity<Cart>()
            .HasOne(c => c.User)
            .WithMany(c => c.Carts)
            .HasForeignKey(c => c.UserId);

            base.OnModelCreating(modelBuilder);
            // definire primary key compus
            modelBuilder.Entity<Ordered_Product>()
            .HasKey(c => new
            {
                c.Id,
                c.OrderId,
                c.ProductId
            });

            // definire relatii cu modelele Order si Product (FK)
            modelBuilder.Entity<Ordered_Product>()
            .HasOne(c => c.Product)
            .WithMany(c => c.Ordered_Products)
            .HasForeignKey(c => c.ProductId);
            modelBuilder.Entity<Ordered_Product>()
            .HasOne(c => c.Order)
            .WithMany(c => c.Ordered_Products)
            .HasForeignKey(c => c.OrderId);

        }
    }
}