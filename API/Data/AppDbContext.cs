using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Define DbSets for your entities
        public DbSet<User> Users { get; set; }
        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<Dish> Dishes { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Category> Categories { get; set; }
        
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configure Order - User relationships (as Customer)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.OrdersPlaced)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Order - User relationships (as Employee/Delivery Person)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Employee)
                .WithMany(u => u.OrdersDelivered)
                .HasForeignKey(o => o.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Order - Restaurant relationship
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Restaurant)
                .WithMany(r => r.Orders)
                .HasForeignKey(o => o.RestaurantId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure OrderItem relationships
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Dish)
                .WithMany(d => d.OrderItems)
                .HasForeignKey(oi => oi.DishId);

            // Configure price columns to have correct precision
            modelBuilder.Entity<Dish>()
                .Property(d => d.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.UnitPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Restaurant>()
                .Property(r => r.DeliveryFee)
                .HasColumnType("decimal(18,2)");

            // Configure many-to-many relationship between Restaurant and User
            modelBuilder.Entity<Restaurant>()
                .HasMany(r => r.Users)
                .WithMany(u => u.Restaurants)
                .UsingEntity(j => j.ToTable("RestaurantUsers"));
        }
    }
}