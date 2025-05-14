using API.AppResponse;
using API.DTOs;
using Microsoft.AspNetCore.Mvc;

public interface IOrderManagementService
{
    Task<AppResponse<OrderDto>> GetOrder(int id);
    Task<AppResponse<List<OrderDto>>> GetAllOrders();
    
}

