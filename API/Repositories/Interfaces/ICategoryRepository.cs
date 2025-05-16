using System;
using API.Models;

namespace API.Repositories.Interfaces;

public interface ICategoryRepository
{
    Task<Category> GetCategoryByIdAsync(int id);
 
}
