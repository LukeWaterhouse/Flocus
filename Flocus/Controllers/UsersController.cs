using Flocus.Domain.Interfaces;
using Flocus.Models;
using Microsoft.AspNetCore.Mvc;

namespace Flocus.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IUserService _userService;


        public UsersController(ILogger<UsersController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        [HttpPost(Name = "Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _userService.RegisterAsync(request.username, request.password, request.isAdmin, request.key);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet(Name = "GetAuthToken")]
        public async Task<IActionResult> GetToken(string username, string password, CancellationToken ct)
        {
            try
            {
                var token = await _userService.GetAuthTokenAsync(username, password);
                return Ok(token);

            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
        }
    }
}
