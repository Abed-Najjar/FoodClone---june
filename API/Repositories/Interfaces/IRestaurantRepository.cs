using System;
using API.Models;

namespace API.Repositories.Interfaces;

public interface IRestaurantRepository
{
    Task<Restaurant> GetRestaurantByIdAsync(int id);
}
