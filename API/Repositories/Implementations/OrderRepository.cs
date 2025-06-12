using API.Data;
using API.Models;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }        
        
        public async Task<List<Order>> GetAllOrdersAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Restaurant)
                .Include(o => o.Employee)
                .Include(o => o.DeliveryAddress)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Dish)
                .ToListAsync();

            return orders;
        }        
        
        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Restaurant)
                .Include(o => o.Employee)
                .Include(o => o.DeliveryAddress)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Dish)
                .FirstOrDefaultAsync(o => o.Id == id);

            return order;
        }        
        
        public async Task<List<Order>> GetOrdersByUserAsync(int userId)
        {
            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.User)
                .Include(o => o.Restaurant)
                .Include(o => o.Employee)
                .Include(o => o.DeliveryAddress)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Dish)
                .ToListAsync();
        
            return orders;
        }

        public async Task<int> GetOrderCountByUserIdAsync(int userId)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId)
                .CountAsync();
        }        
        
        public async Task<List<Order>> GetOrdersByRestaurantAsync(int restaurantId)
        {
            var orders = await _context.Orders
                .Where(o => o.RestaurantId == restaurantId)
                .Include(o => o.User)
                .Include(o => o.Restaurant)
                .Include(o => o.Employee)
                .Include(o => o.DeliveryAddress)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Dish)
                .ToListAsync();

            return orders;
        }        
        
        public async Task<List<Order>> GetOrdersByEmployeeAsync(int employeeId)
        {
            var orders = await _context.Orders
                .Where(o => o.EmployeeId == employeeId)
                .Include(o => o.User)
                .Include(o => o.Restaurant)
                .Include(o => o.Employee)
                .Include(o => o.DeliveryAddress)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Dish)
                .ToListAsync();

            return orders;
        }
        
        public async Task<bool> UpdateOrderStatusAsync(int id, string status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return false;
            }

            order.Status = status;
            _context.Orders.Update(order);

            return true;
        }

        public async Task<bool> DeleteOrderAsync(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);
            
            if (order == null)
            {
                return false;
            }

            _context.Orders.Remove(order);
            
            return true;
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            
            // Return the order with all navigation properties loaded
            return await GetOrderByIdAsync(order.Id) ?? order;
        }

        public async Task<Dish?> GetDishByIdAsync(int dishId)
        {
            return await _context.Dishes
                .Include(d => d.Restaurant)
                .FirstOrDefaultAsync(d => d.Id == dishId);
        }        
        
        public async Task<Restaurant?> GetRestaurantByIdAsync(int restaurantId)
        {
            return await _context.Restaurants
                .FirstOrDefaultAsync(r => r.Id == restaurantId);
        }

        public async Task<Address?> GetAddressByIdAsync(int addressId, int userId)
        {
            return await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId);
        }
    }
}
