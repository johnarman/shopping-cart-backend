using ShoppingCart.Entity.Model;
using Microsoft.EntityFrameworkCore;
using ShoppingCart.Infrastructure.Data.Configurations;

namespace ShoppingCart.Infrastructure.Data
{
    public class ShoppingCartContext : DbContext
    {
        public ShoppingCartContext(DbContextOptions<ShoppingCartContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> ShoppingCarts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
            modelBuilder.ApplyConfiguration(new ShoppingCartConfiguration());

            // Seed data for Users
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "admin", Password = BCrypt.Net.BCrypt.HashPassword("admin123") },
                new User { Id = 2, Username = "testuser", Password = BCrypt.Net.BCrypt.HashPassword("test123") }
            );

            // Seed data for Products
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Laptop", Price = 999.99m, Stock = 10, ReservedQuantity=0, ImageUrl = "https://images.unsplash.com/photo-1517336714731-489689fd1ca8" },
                new Product { Id = 2, Name = "Smartphone", Price = 499.99m, Stock = 20, ReservedQuantity = 0, ImageUrl = "https://images.unsplash.com/photo-1517336714731-489689fd1ca8" },
                new Product { Id = 3, Name = "Headphone", Price = 99.99m, Stock = 15, ReservedQuantity = 0, ImageUrl = "https://images.unsplash.com/photo-1517336714731-489689fd1ca8" },
                new Product { Id = 4, Name = "Toy", Price = 99.99m, Stock = 15, ReservedQuantity = 0, ImageUrl = "https://images.unsplash.com/photo-1517336714731-489689fd1ca8" },
                new Product { Id = 5, Name = "Tablet", Price = 99.99m, Stock = 15, ReservedQuantity = 0, ImageUrl = "https://images.unsplash.com/photo-1517336714731-489689fd1ca8" },
                new Product { Id = 6, Name = "Mobile", Price = 99.99m, Stock = 15, ReservedQuantity = 0, ImageUrl = "https://images.unsplash.com/photo-1517336714731-489689fd1ca8" },
                new Product { Id = 7, Name = "Sofa", Price = 99.99m, Stock = 15, ReservedQuantity = 0, ImageUrl = "https://images.unsplash.com/photo-1517336714731-489689fd1ca8" },
                new Product { Id = 8, Name = "Television", Price = 99.99m, Stock = 15, ReservedQuantity = 0, ImageUrl = "https://images.unsplash.com/photo-1517336714731-489689fd1ca8" }
            );
        }
    }
}
