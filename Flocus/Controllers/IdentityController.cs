using Flocus.Identity.Interfaces;
using Flocus.Models.Errors;
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

    [HttpPost("register", Name = "Register")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequestDto request, CancellationToken ct)
    {
        if (request.isAdmin && request.key == null)
        {
            return BadRequest(new ErrorsDto(new List<ErrorDto>
            {
                new ErrorDto(StatusCodes.Status400BadRequest, "Must provide key when creating admin")
            }));
        }

        if (!ModelState.IsValid) //TODO: consider moving this to a common function
        {
            var errors = new List<ErrorDto>();
            foreach (var error in ModelState)
            {
                if (error.Value != null && error.Value.Errors.Any())
                {
                    errors.Add(new ErrorDto(StatusCodes.Status400BadRequest, error.Value.Errors.First().ErrorMessage));
                }
            }
            return BadRequest(new ErrorsDto(errors));
        }

        await _identityService.RegisterAsync(request.username, request.password, request.emailAddress, request.isAdmin, request.key);
        return Ok();
    }

    [HttpPost("getToken", Name = "GetToken")]
    public async Task<IActionResult> GetTokenAsync([FromForm] string username, [FromForm] string password, CancellationToken ct)
    {
        var token = await _identityService.GetAuthTokenAsync(username, password);
        return token != null ? Ok(token) : StatusCode(500, "Error generating token");
    }
}
