using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    // DTO for returning order data to clients
    public class OrderDto
    {
        public int Id { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        
        // Customer information
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        
        // Restaurant information
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; } = string.Empty;
        
        // Delivery employee information
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        
        // Order items
        public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
    }

    // DTO for order items
    public class OrderItemDto
    {
        public int Id { get; set; }
        public int DishId { get; set; }
        public string DishName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class OrderCreateDto
    {
        // No ID field - will be auto-generated
        public string? PaymentMethod { get; set; }

        [Required]
        public required int RestaurantId { get; set; }

        [Required]
        public required List<OrderItemCreateDto> OrderItems { get; set; } = new();
  
    }

    public class OrderItemCreateDto
    {
        // No ID field - will be auto-generated
        public int DishId { get; set; }
        public int Quantity { get; set; }
    }

    // DTO for creating a new order
    public class CreateOrderDto
    {
        public decimal TotalAmount { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Status { get; set; }
        public int UserId { get; set; }
        public int RestaurantId { get; set; }
        public int EmployeeId { get; set; }
        
    }

    // DTO for creating order items
    public class CreateOrderItemDto
    {
        [Required]
        public int DishId { get; set; }
        
        [Required, Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }

    // DTO for updating an existing order
    public class UpdateOrderDto
    {
        public int? EmployeeId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
    }

    // DTO for filtering and pagination of orders
    public class OrderQueryDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string Status { get; set; } = string.Empty;
        public int? UserId { get; set; }
        public int? RestaurantId { get; set; }
        public int? EmployeeId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
