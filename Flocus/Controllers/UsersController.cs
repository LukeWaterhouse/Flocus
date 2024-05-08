using Microsoft.AspNetCore.Mvc;

namespace Flocus.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;

        public UsersController(ILogger<UsersController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "CreateUser")]
        public async Task<IActionResult> CreateUser()
        {

            return Ok();
    
        }
    }
}