using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Dish> Dishes { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDish> OrderDishes { get; set; } // Fixed: Changed from OrderDish (singular) to OrderDishes (plural)

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User-Order relationships
            modelBuilder.Entity<User>()
                .HasMany(u => u.OrdersPlaced)
                .WithOne(o => o.User)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.OrdersDelivered)
                .WithOne(o => o.Employee)
                .HasForeignKey(o => o.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Restaurant-Order relationship
            modelBuilder.Entity<Restaurant>()
                .HasMany(r => r.Orders)
                .WithOne(o => o.Restaurant)
                .HasForeignKey(o => o.RestaurantId);

            // Configure Order-OrderDish relationship
            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId);

            // Configure Dish-OrderDish relationship
            modelBuilder.Entity<Dish>()
                .HasMany(d => d.OrderItems) // Update property name on Dish model if needed
                .WithOne(oi => oi.Dish)
                .HasForeignKey(oi => oi.DishId);

            // Configure many-to-many relationships between User and Restaurant
            modelBuilder.Entity<User>()
                .HasMany(u => u.Restaurants)
                .WithMany(r => r.Users);

            // Configure OrderDish entity explicitly to ensure consistent table naming
            modelBuilder.Entity<OrderDish>().ToTable("OrderDishes");
        }

        // Here's a summary of the relationships in your model:
        /*
        1. User - Restaurant (Many-to-Many)
           - Users can be associated with multiple restaurants 
           - Restaurants can have multiple users

        2. User - Order (One-to-Many, two relationships)
           - A User can place multiple orders (as a customer)
           - A User can deliver multiple orders (as an employee)
           - Each Order has exactly one User as customer and one User as delivery employee

        3. Restaurant - Order (One-to-Many)
           - A Restaurant can have multiple Orders
           - Each Order belongs to exactly one Restaurant

        4. Restaurant - Category (One-to-Many)
           - A Restaurant can have multiple Categories
           - Each Category belongs to exactly one Restaurant

        5. Restaurant - Dish (One-to-Many)
           - A Restaurant can have multiple Dishes
           - Each Dish belongs to exactly one Restaurant

        6. Order - OrderItem (One-to-Many)
           - An Order can have multiple OrderItems
           - Each OrderItem belongs to exactly one Order

        7. Dish - OrderItem (One-to-Many)
           - A Dish can be referenced by multiple OrderItems
           - Each OrderItem references exactly one Dish
        */
    }
}
