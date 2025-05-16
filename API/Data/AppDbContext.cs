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
        public DbSet<OrderDish> OrderDishes { get; set; }
        public DbSet<RestaurantsCategories> RestaurantsCategories { get; set; }

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
                
            // Configure many-to-many relationship between Restaurant and Category using the join entity
            modelBuilder.Entity<RestaurantsCategories>()
                .HasKey(rc => new { rc.RestaurantId, rc.CategoryId });
                
            modelBuilder.Entity<RestaurantsCategories>()
                .HasOne(rc => rc.Restaurant)
                .WithMany(r => r.RestaurantsCategories)
                .HasForeignKey(rc => rc.RestaurantId);
                
            modelBuilder.Entity<RestaurantsCategories>()
                .HasOne(rc => rc.Category)
                .WithMany(c => c.RestaurantsCategories)
                .HasForeignKey(rc => rc.CategoryId);

            // Configure OrderDish entity explicitly to ensure consistent table naming
            modelBuilder.Entity<OrderDish>().ToTable("OrderDishes");
            
            // Enhancement 1: Add direct relationship between Restaurant and Dishes
            modelBuilder.Entity<Restaurant>()
                .HasMany(r => r.Dishes)
                .WithOne(d => d.Restaurant)
                .HasForeignKey(d => d.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);
                
            // Enhancement 2: Add indexes on frequently queried foreign keys
            modelBuilder.Entity<Order>()
                .HasIndex(o => o.UserId);
                
            modelBuilder.Entity<Order>()
                .HasIndex(o => o.RestaurantId);
                
            // Enhancement 3: Make EmployeeId nullable since an order might not be assigned immediately
            modelBuilder.Entity<Order>()
                .Property(o => o.EmployeeId)
                .IsRequired(false);
                
            // Enhancement 4: Add uniqueness constraint for dish names within a restaurant
            modelBuilder.Entity<Dish>()
                .HasIndex(d => new { d.RestaurantId, d.Name })
                .IsUnique();
                
            // Enhancement 5: Add timestamps for auditing
            modelBuilder.Entity<Order>()
                .Property<DateTime>("CreatedAt")
                .HasDefaultValueSql("GETDATE()");
                
            modelBuilder.Entity<Order>()
                .Property<DateTime>("UpdatedAt")
                .HasDefaultValueSql("GETDATE()");
        }
        
        /*
        Summary of Relationships:
        
        User:
        - One User -> Many Orders (as OrdersPlaced)
        - One User -> Many Orders (as OrdersDelivered, when user is an employee)
        - Many Users <-> Many Restaurants
        
        Restaurant:
        - One Restaurant -> Many Orders
        - Many Restaurants <-> Many Users
        - Many Restaurants <-> Many Categories (through RestaurantsCategories join table)
        
        Order:
        - One Order -> Many OrderDishes (as OrderItems)
        - One Order <- One User (as customer)
        - One Order <- One User (as employee/delivery person)
        - One Order <- One Restaurant
        
        Dish:
        - One Dish -> Many OrderDishes (as OrderItems)
        
        Category:
        - Many Categories <-> Many Restaurants (through RestaurantsCategories join table)
        
        RestaurantsCategories:
        - Join table for Many-to-Many relationship between Restaurant and Category
        */
    }
}
