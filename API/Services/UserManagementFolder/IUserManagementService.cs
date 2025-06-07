using API.AppResponse;
using API.DTOs;
using Microsoft.AspNetCore.Mvc;

public interface IUserManagementService
{
    Task<AppResponse<UserDto>> CreateUser([FromBody] UserInputDto dto);
    Task<AppResponse<PagedResultDto<UserDto>>> GetAllUsers(PaginationDto? paginationDto = null);
    Task<AppResponse<UserDto>> GetUser(int id);
    Task<AppResponse<bool>> DeleteUser(int id);
    Task<AppResponse<UserDto>> UpdateUser(int id, [FromBody] UserInputDto dto);
}
