using API.AppResponse;
using API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{

    [ApiController]
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    public class UserManagementController : ControllerBase
    {
        private readonly IUserManagementService _userManagementService;

        public UserManagementController(IUserManagementService userManagementService)
        {
            _userManagementService = userManagementService;
        }

        [HttpPost("create")]
        public async Task<AppResponse<UserDto>> CreateUser([FromBody] UserInputDto dto)
        {
            return await _userManagementService.CreateUser(dto);
        }

        [HttpGet("getAll")]
        public async Task<AppResponse<List<UserDto>>> GetAllUsers()
        {
            return await _userManagementService.GetAllUsers();
        }

        [HttpGet("get/{id}")]
        public async Task<AppResponse<UserDto>> GetUser(int id)
        {
            return await _userManagementService.GetUser(id);
        }

        [HttpDelete("delete/{id}")]
        public async Task<AppResponse<bool>> DeleteUser(int id)
        {
            return await _userManagementService.DeleteUser(id);
        }

        [HttpPut("update/{id}")]
        public async Task<AppResponse<UserDto>> UpdateUser(int id, [FromBody] UserInputDto dto)
        {
            return await _userManagementService.UpdateUser(id, dto);
        }
    }
}