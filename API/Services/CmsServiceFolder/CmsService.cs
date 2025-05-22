using API.DTOs;
using API.AppResponse;
using API.Repositories.Interfaces;
using API.Models;
using Microsoft.EntityFrameworkCore;
using API.Data;

namespace API.Services.CmsServiceFolder
{
    public class CmsService : ICmsService
    {
        private readonly ICmsRepository _cmsRepository;
        private readonly ILogger<CmsService> _logger;

        public CmsService(ICmsRepository cmsRepository, ILogger<CmsService> logger)
        {
            _cmsRepository = cmsRepository;
            _logger = logger;
        }
    
        public async Task<AppResponse<List<CategoriesDto>>> GetAllCategoriesAsync()
        {
            var categories = await _cmsRepository.GetAllCategoriesAsync();
            if (categories == null || !categories.Any())
            {
                return new AppResponse<List<CategoriesDto>>(null, "No categories found.", 200, false);
            }
            var categoryDtos = categories.Select(c => new CategoriesDto
            {
                Id = c.Id,
                Name = c.Name,
                ImageUrl = c.ImageUrl,
            }).ToList();
            

            return new AppResponse<List<CategoriesDto>>(categoryDtos);
        }

        public async Task<AppResponse<List<RestaurantDto>>> GetAllRestaurantsAsync()
        {
            var restaurants = await _cmsRepository.GetAllRestaurantsAsync();
            if (restaurants == null || !restaurants.Any())
            {
                return new AppResponse<List<RestaurantDto>>(null, "No restaurants found.", 200, false);
            }
            var restaurantDtos = restaurants.Select(r => new RestaurantDto
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                LogoUrl = r.LogoUrl,
                CoverImageUrl = r.CoverImageUrl,
                Address = r.Address,
                PhoneNumber = r.PhoneNumber,
                Email = r.Email,
                IsOpen = r.IsOpen,
                Rating = r.Rating,

            }).ToList();

            return new AppResponse<List<RestaurantDto>>(restaurantDtos);
        }


        public async Task<AppResponse<List<AdminRestaurantDishDto>>> GetAllDishesAsync()
        {
            var dishes = await _cmsRepository.GetAllDishesAsync();
            if (dishes == null || !dishes.Any())
            {
                return new AppResponse<List<AdminRestaurantDishDto>>(null, "No dishes found.", 200, false);
            }
            var dishDtos = dishes.Select(d => new AdminRestaurantDishDto
            {
                Id = d.Id,
                Name = d.Name,
                Description = d.Description,
                Price = d.Price,
                ImageUrl = d.ImageUrl,
                RestaurantName = d.Restaurant.Name,
                RestaurantId = d.RestaurantId,
                CategoryName = d.Category?.Name,
                CategoryId = d.CategoryId
            }).ToList();

            return new AppResponse<List<AdminRestaurantDishDto>>(dishDtos);
        }

        public async Task<AppResponse<List<UserDto>>> GetAllUsersAsync()
        {
            var users = await _cmsRepository.GetAllUsersAsync();
            if (users == null || !users.Any())
            {
                return new AppResponse<List<UserDto>>(null, "No users found.", 200, false);
            }
            var userDtos = users.Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.UserName,
                Email = u.Email,
                Phonenumber = u.PhoneNumber,
                Rolename = u.Role.ToString(),
                Createdat = u.CreatedAt,
                Address = u.Address.ToList(),
            }).ToList();

            return new AppResponse<List<UserDto>>(userDtos);
        }

        public async Task<AppResponse<CategoryDto>> UpdateCategoryAsync(int id, CategoryDto categoryDto)
        {
            var category = await _cmsRepository.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return new AppResponse<CategoryDto>(null, "Category not found.", 404, false);
            }

            category.Name = categoryDto.Name;
            category.Description = categoryDto.Description;
            category.ImageUrl = categoryDto.ImageUrl;

            var updatedCategory = await _cmsRepository.UpdateCategoryAsync(category);
            if (updatedCategory == null)
            {
                return new AppResponse<CategoryDto>(null, "Failed to update category.", 500, false);
            }

            return new AppResponse<CategoryDto>(new CategoryDto
            {
                Id = updatedCategory.Id,
                Name = updatedCategory.Name,
                Description = updatedCategory.Description,
                ImageUrl = updatedCategory.ImageUrl,
            });
        }

        public async Task<AppResponse<bool>> DeleteCategoryAsync(int id)
        {
            var category = await _cmsRepository.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return new AppResponse<bool>(false, "Category not found.", 404, false);
            }

            var result = await _cmsRepository.DeleteCategoryAsync(id);
            if (!result)
            {
                return new AppResponse<bool>(false, "Failed to delete category.", 500, false);
            }

            return new AppResponse<bool>(true, "Category deleted successfully.", 200, true);
        }

        public async Task<AppResponse<AdminRestaurantDto>> UpdateRestaurantAsync(int id, AdminRestaurantUpdateDto restaurantDto)
        {
            var restaurant = await _cmsRepository.GetRestaurantByIdAsync(id);
            if (restaurant == null)
            {
                return new AppResponse<AdminRestaurantDto>(null, "Restaurant not found.", 404, false);
            }

            restaurant.Name = restaurantDto.Name!;
            restaurant.Description = restaurantDto.Description!;
            restaurant.LogoUrl = restaurantDto.LogoUrl!;
            restaurant.CoverImageUrl = restaurantDto.CoverImageUrl!;
            restaurant.Address = restaurantDto.Address!;
            restaurant.PhoneNumber = restaurantDto.PhoneNumber!;
            restaurant.Email = restaurantDto.Email!;

            var updatedRestaurant = await _cmsRepository.UpdateRestaurantAsync(restaurant);
            if (updatedRestaurant == null)
            {
                return new AppResponse<AdminRestaurantDto>(null, "Failed to update restaurant.", 500, false);
            }

            return new AppResponse<AdminRestaurantDto>(new AdminRestaurantDto
            {
                Id = updatedRestaurant.Id,
                Name = updatedRestaurant.Name,
                Description = updatedRestaurant.Description,
                LogoUrl = updatedRestaurant.LogoUrl,
                CoverImageUrl = updatedRestaurant.CoverImageUrl,
                Address = updatedRestaurant.Address,
                PhoneNumber = updatedRestaurant.PhoneNumber,
                Email = updatedRestaurant.Email,
                IsOpen = updatedRestaurant.IsOpen,
                Rating = updatedRestaurant.Rating,

            });
        }

        public async Task<AppResponse<bool>> DeleteRestaurantAsync(int id)
        {
            var restaurant = await _cmsRepository.GetRestaurantByIdAsync(id);
            if (restaurant == null)
            {
                return new AppResponse<bool>(false, "Restaurant not found.", 404, false);
            }

            var result = await _cmsRepository.DeleteRestaurantAsync(id);
            if (!result)
            {
                return new AppResponse<bool>(false, "Failed to delete restaurant.", 500, false);
            }

            return new AppResponse<bool>(true, "Restaurant deleted successfully.", 200, true);
        }

        public async Task<AppResponse<AdminRestaurantDishDto>> UpdateDishAsync(int id, AdminRestaurantDishDto dishDto)
        {
            var dish = await _cmsRepository.GetDishByIdAsync(id);
            if (dish == null)
            {
                return new AppResponse<AdminRestaurantDishDto>(null, "Dish not found.", 404, false);
            }

            dish.Name = dishDto.Name;
            dish.Description = dishDto.Description;
            dish.Price = dishDto.Price;
            dish.ImageUrl = dishDto.ImageUrl;

            var updatedDish = await _cmsRepository.UpdateDishAsync(dish);
            if (updatedDish == null)
            {
                return new AppResponse<AdminRestaurantDishDto>(null, "Failed to update dish.", 500, false);
            }

            return new AppResponse<AdminRestaurantDishDto>(new AdminRestaurantDishDto
            {
                Id = updatedDish.Id,
                Name = updatedDish.Name,
                Description = updatedDish.Description,
                Price = updatedDish.Price,
                ImageUrl = updatedDish.ImageUrl,
                RestaurantName = updatedDish.Restaurant.Name,
                RestaurantId = updatedDish.RestaurantId,
                CategoryName = updatedDish.Category?.Name,
                CategoryId = updatedDish.CategoryId
            });
        }

        public async Task<AppResponse<bool>> DeleteDishAsync(int id)
        {
            var dish = await _cmsRepository.GetDishByIdAsync(id);
            if (dish == null)
            {
                return new AppResponse<bool>(false, "Dish not found.", 404, false);
            }

            var result = await _cmsRepository.DeleteDishAsync(id);
            if (!result)
            {
                return new AppResponse<bool>(false, "Failed to delete dish.", 500, false);
            }

            return new AppResponse<bool>(true, "Dish deleted successfully.", 200, true);
        }

        public async Task<AppResponse<bool>> DeleteUserAsync(int id)
        {
            var user = await _cmsRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                return new AppResponse<bool>(false, "User not found.", 404, false);
            }

            var result = await _cmsRepository.DeleteUserAsync(id);
            if (!result)
            {
                return new AppResponse<bool>(false, "Failed to delete user.", 500, false);
            }

            return new AppResponse<bool>(true, "User deleted successfully.", 200, true);
        }

        public async Task<AppResponse<bool>> UpdateOrderStatusAsync(int id, string status)
        {
            var order = await _cmsRepository.GetOrderByIdAsync(id);
            if (order == null)
            {
                return new AppResponse<bool>(false, "Order not found.", 404, false);
            }

            order.Status = status;

            var result = await _cmsRepository.UpdateOrderStatusAsync(id, status);
            if (result == null)
            {
                return new AppResponse<bool>(false, "Failed to update order status.", 500, false);
            }

            return new AppResponse<bool>(true, "Order status updated successfully.", 200, true);
        }

        public async Task<AppResponse<List<OrderDto>>> GetAllOrdersAsync()
        {
            var orders = await _cmsRepository.GetAllOrdersAsync();
            if (orders == null || !orders.Any())
            {
                return new AppResponse<List<OrderDto>>(null, "No orders found.", 200, false);
            }
            var orderDtos = orders.Select(o => new OrderDto
            {
                Id = o.Id,
                UserId = o.UserId,
                RestaurantId = o.RestaurantId,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                PaymentMethod = o.PaymentMethod,
                OrderDate = o.CreatedAt,
                UserName = o.User.UserName,
                RestaurantName = o.Restaurant.Name,
                EmployeeId = o.EmployeeId ?? 0,
                EmployeeName = o.Employee != null ? o.Employee.UserName : "Not Assigned",
                OrderItems = o.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    DishId = oi.DishId,
                    DishName = oi.Dish.Name,
                    UnitPrice = oi.UnitPrice,
                    Quantity = oi.Quantity,
                    TotalPrice = oi.Quantity * oi.UnitPrice
                }).ToList(),

            }).ToList();

            return new AppResponse<List<OrderDto>>(orderDtos);
        }

        public async Task<AppResponse<OrderDto>> GetOrderByIdAsync(int id)
        {
            var order = await _cmsRepository.GetOrderByIdAsync(id);
            if (order == null)
            {
                return new AppResponse<OrderDto>(null, "Order not found.", 404, false);
            }

            var orderDto = new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                RestaurantId = order.RestaurantId,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                PaymentMethod = order.PaymentMethod,
                OrderDate = order.CreatedAt,
                UserName = order.User.UserName,
                RestaurantName = order.Restaurant.Name,
                EmployeeId = order.EmployeeId ?? 0,
                EmployeeName = order.Employee != null ? order.Employee.UserName : "Not Assigned",
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    DishId = oi.DishId,
                    DishName = oi.Dish.Name,
                    UnitPrice = oi.UnitPrice,
                    Quantity = oi.Quantity,
                    TotalPrice = oi.Quantity * oi.UnitPrice
                }).ToList(),
            };

            return new AppResponse<OrderDto>(orderDto);
        }

        public async Task<AppResponse<List<OrderDto>>> GetOrdersByUserAsync(int userId)
        {
            var orders = await _cmsRepository.GetOrdersByUserAsync(userId);
            if (orders == null || !orders.Any())
            {
                return new AppResponse<List<OrderDto>>(null, "No orders found for this user.", 200, false);
            }
            var orderDtos = orders.Select(o => new OrderDto
            {
                Id = o.Id,
                UserId = o.UserId,
                RestaurantId = o.RestaurantId,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                PaymentMethod = o.PaymentMethod,
                OrderDate = o.CreatedAt,
                UserName = o.User.UserName,
                RestaurantName = o.Restaurant.Name,
                EmployeeId = o.EmployeeId ?? 0,
                EmployeeName = o.Employee != null ? o.Employee.UserName : "Not Assigned",
                OrderItems = o.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    DishId = oi.DishId,
                    DishName = oi.Dish.Name,
                    UnitPrice = oi.UnitPrice,
                    Quantity = oi.Quantity,
                    TotalPrice = oi.Quantity * oi.UnitPrice
                }).ToList(),

            }).ToList();

            return new AppResponse<List<OrderDto>>(orderDtos);
        }

        public async Task<AppResponse<List<OrderDto>>> GetOrdersByRestaurantAsync(int restaurantId)
        {
            var orders = await _cmsRepository.GetOrdersByRestaurantAsync(restaurantId);
            if (orders == null || !orders.Any())
            {
                return new AppResponse<List<OrderDto>>(null, "No orders found for this restaurant.", 200, false);
            }
            var orderDtos = orders.Select(o => new OrderDto
            {
                Id = o.Id,
                UserId = o.UserId,
                RestaurantId = o.RestaurantId,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                PaymentMethod = o.PaymentMethod,
                OrderDate = o.CreatedAt,
                UserName = o.User.UserName,
                RestaurantName = o.Restaurant.Name,
                EmployeeId = o.EmployeeId ?? 0,
                EmployeeName = o.Employee != null ? o.Employee.UserName : "Not Assigned",
                OrderItems = o.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    DishId = oi.DishId,
                    DishName = oi.Dish.Name,
                    UnitPrice = oi.UnitPrice,
                    Quantity = oi.Quantity,
                    TotalPrice = oi.Quantity * oi.UnitPrice
                }).ToList(),

            }).ToList();

            return new AppResponse<List<OrderDto>>(orderDtos);
        }

        public async Task<AppResponse<List<OrderDto>>> GetOrdersByEmployeeAsync(int employeeId)
        {
            var orders = await _cmsRepository.GetOrdersByEmployeeAsync(employeeId);
            if (orders == null || !orders.Any())
            {
                return new AppResponse<List<OrderDto>>(null, "No orders found for this employee.", 200, false);
            }
            var orderDtos = orders.Select(o => new OrderDto
            {
                Id = o.Id,
                UserId = o.UserId,
                RestaurantId = o.RestaurantId,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                PaymentMethod = o.PaymentMethod,
                OrderDate = o.CreatedAt,
                UserName = o.User.UserName,
                RestaurantName = o.Restaurant.Name,
                EmployeeId = o.EmployeeId ?? 0,
                EmployeeName = o.Employee != null ? o.Employee.UserName : "Not Assigned",
                OrderItems = o.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    DishId = oi.DishId,
                    DishName = oi.Dish.Name,
                    UnitPrice = oi.UnitPrice,
                    Quantity = oi.Quantity,
                    TotalPrice = oi.Quantity * oi.UnitPrice
                }).ToList(),

            }).ToList();

            return new AppResponse<List<OrderDto>>(orderDtos);
        }

        public async Task<AppResponse<AdminRestaurantDto>> CreateRestaurantAsync(AdminRestaurantCreateDto restaurantDto)
        {

            var restaurant = new Restaurant
            {
                Name = restaurantDto.Name,
                Description = restaurantDto.Description,
                LogoUrl = restaurantDto.LogoUrl,
                CoverImageUrl = restaurantDto.CoverImageUrl,
                Address = restaurantDto.Address,
                PhoneNumber = restaurantDto.PhoneNumber,
                Email = restaurantDto.Email,
            };

            var createdRestaurant = await _cmsRepository.CreateRestaurantAsync(restaurant);
            if (createdRestaurant == null)
            {
                return new AppResponse<AdminRestaurantDto>(null, "Failed to create restaurant.", 500, false);
            }
            return new AppResponse<AdminRestaurantDto>(new AdminRestaurantDto
            {
                Id = createdRestaurant.Id,
                Name = createdRestaurant.Name,
                Description = createdRestaurant.Description,
                LogoUrl = createdRestaurant.LogoUrl,
                CoverImageUrl = createdRestaurant.CoverImageUrl,
                Address = createdRestaurant.Address,
                PhoneNumber = createdRestaurant.PhoneNumber,
                Email = createdRestaurant.Email,
                IsOpen = createdRestaurant.IsOpen,
                Rating = createdRestaurant.Rating,

            });

        }

        public async Task<AppResponse<AdminRestaurantDto>> UpdateRestaurantAsync(int id, AdminRestaurantDto restaurantDto)
        {
            var restaurant = await _cmsRepository.GetRestaurantByIdAsync(id);
            if (restaurant == null)
            {
                return new AppResponse<AdminRestaurantDto>(null, "Restaurant not found.", 404, false);
            }

            restaurant.Name = restaurantDto.Name!;
            restaurant.Description = restaurantDto.Description!;
            restaurant.LogoUrl = restaurantDto.LogoUrl!;
            restaurant.CoverImageUrl = restaurantDto.CoverImageUrl!;
            restaurant.Address = restaurantDto.Address!;
            restaurant.PhoneNumber = restaurantDto.PhoneNumber!;
            restaurant.Email = restaurantDto.Email!;

            var updatedRestaurant = await _cmsRepository.UpdateRestaurantAsync(restaurant);
            if (updatedRestaurant == null)
            {
                return new AppResponse<AdminRestaurantDto>(null, "Failed to update restaurant.", 500, false);
            }

            return new AppResponse<AdminRestaurantDto>(new AdminRestaurantDto
            {
                Id = updatedRestaurant.Id,
                Name = updatedRestaurant.Name,
                Description = updatedRestaurant.Description,
                LogoUrl = updatedRestaurant.LogoUrl,
                CoverImageUrl = updatedRestaurant.CoverImageUrl,
                Address = updatedRestaurant.Address,
                PhoneNumber = updatedRestaurant.PhoneNumber,
                Email = updatedRestaurant.Email,
                IsOpen = updatedRestaurant.IsOpen,
                Rating = updatedRestaurant.Rating,

            });
        }

        public async Task<AppResponse<AdminRestaurantDishDto>> CreateDishAsync(CreateDishDto dishDto)
        {
            // Check if restaurant exists
            var restaurant = await _cmsRepository.GetRestaurantByIdAsync(dishDto.RestaurantId);
            if (restaurant == null)
            {
                return new AppResponse<AdminRestaurantDishDto>(null, "Restaurant not found.", 404, false);
            }

            // Check if category exists if provided
            Category? category = null;
            if (dishDto.CategoryId.HasValue)
            {
                category = await _cmsRepository.GetCategoryByIdAsync(dishDto.CategoryId.Value);
                if (category == null)
                {
                    return new AppResponse<AdminRestaurantDishDto>(null, "Category not found.", 404, false);
                }
            }

            // Create dish entity
            var dish = new Dish
            {
                Name = dishDto.Name,
                Description = dishDto.Description,
                Price = dishDto.Price,
                ImageUrl = dishDto.ImageUrl,
                RestaurantId = dishDto.RestaurantId,
                Restaurant = restaurant, // Set the navigation property
                CategoryId = dishDto.CategoryId,
                Quantity = 0 // Set default quantity
            };

            // Save dish
            var createdDish = await _cmsRepository.CreateDishAsync(dish);
            if (createdDish == null)
            {
                return new AppResponse<AdminRestaurantDishDto>(null, "Failed to create dish.", 500, false);
            }

            // Return DTO with complete info
            return new AppResponse<AdminRestaurantDishDto>(
                new AdminRestaurantDishDto
                {
                    Id = createdDish.Id,
                    Name = createdDish.Name,
                    Description = createdDish.Description,
                    Price = createdDish.Price,
                    ImageUrl = createdDish.ImageUrl,
                    RestaurantId = createdDish.RestaurantId,
                    RestaurantName = restaurant.Name,
                    CategoryId = createdDish.CategoryId,
                    CategoryName = category?.Name
                },
                "Dish created successfully.",
                201,
                true
            );
        }

        public async Task<AppResponse<AdminRestaurantDto>> GetRestaurantByIdAsync(int id)
        {
            var restaurant = await _cmsRepository.GetRestaurantByIdAsync(id);
            if (restaurant == null)
            {
                return new AppResponse<AdminRestaurantDto>(null, "Restaurant not found.", 404, false);
            }

            var restaurantDto = new AdminRestaurantDto
            {
                Id = restaurant.Id,
                Name = restaurant.Name,
                Description = restaurant.Description,
                LogoUrl = restaurant.LogoUrl,
                CoverImageUrl = restaurant.CoverImageUrl,
                Address = restaurant.Address,
                PhoneNumber = restaurant.PhoneNumber,
                Email = restaurant.Email,
                IsOpen = restaurant.IsOpen,
                Rating = restaurant.Rating
            };

            return new AppResponse<AdminRestaurantDto>(restaurantDto);
        }
    }
    
    
    
}
