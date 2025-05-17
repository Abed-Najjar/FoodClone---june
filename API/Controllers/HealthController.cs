using API.AppResponse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public ActionResult<AppResponse<string>> Get()
        {
            return new AppResponse<string>("OK", "API is healthy", 200, true);
        }
    }
}
