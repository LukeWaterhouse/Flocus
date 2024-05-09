using Flocus.Identity.Interfaces;
using Flocus.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Flocus.Controllers;

[ApiController]
[Route("[controller]")]
public class IdentityController : ControllerBase
{
    private readonly ILogger<IdentityController> _logger;
    private readonly IIdentityService _identityService;


    public IdentityController(ILogger<IdentityController> logger, IIdentityService identityService)
    {
        _logger = logger;
        _identityService = identityService;
    }

    [HttpPost(Name = "Register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _identityService.RegisterAsync(request.username, request.password, request.isAdmin, request.key);
        return Ok();

    }

    [HttpGet(Name = "GetAuthToken")]
    public async Task<IActionResult> GetToken(string username, string password, CancellationToken ct)
    {
        var token = await _identityService.GetAuthTokenAsync(username, password);
        return Ok(token);
    }
}
