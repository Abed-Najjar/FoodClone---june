using API.AppResponse;
using API.DTOs;
using API.Models;

namespace API.Repositories.Interfaces; 

public interface IOrderRepository
{
    Task<List<Order>> GetAllOrdersAsync();
    Task<Order?> GetOrderByIdAsync(int id);
    Task<List<Order>> GetOrdersByUserAsync(int userId);
    Task<int> GetOrderCountByUserIdAsync(int userId);
    Task<List<Order>> GetOrdersByRestaurantAsync(int restaurantId);
    Task<List<Order>> GetOrdersByEmployeeAsync(int employeeId);
    Task<bool> UpdateOrderStatusAsync(int id, string status);
    Task<Order> CreateOrderAsync(Order order);
    Task<bool> DeleteOrderAsync(int id);
    Task<Dish?> GetDishByIdAsync(int dishId);
    Task<Restaurant?> GetRestaurantByIdAsync(int restaurantId);
    Task<Address?> GetAddressByIdAsync(int addressId, int userId);
}
