using API.AppResponse;
using API.DTOs;
using Microsoft.AspNetCore.Mvc;

public interface IUserManagementService
{
    Task<AppResponse<UserDto>> CreateUser([FromBody] UserInputDto dto);
    Task<AppResponse<List<UserDto>>> GetAllUsers();
    Task<AppResponse<UserDto>> GetUser(int id);
    Task<AppResponse<bool>> DeleteUser(int id);
    Task<AppResponse<UserDto>> UpdateUser(int id, [FromBody] UserInputDto dto);
}
