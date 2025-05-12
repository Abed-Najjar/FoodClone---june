using API.AppResponse;
using API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        // [Authorize(Roles = "Admin","User")]
        // // [HttpGet("getAll")]
        // // public List<AppResponse<UserDto>> GetAllUsers()
        // // {
            
        // // }
    }
}