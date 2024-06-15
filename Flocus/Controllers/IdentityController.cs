using Flocus.Identity.Interfaces;
using Flocus.Models.Errors;
using Flocus.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;
using System.Security.Claims;

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

    [Authorize]
    [HttpDelete("deleteUser", Name = "deleteUser")]
    public async Task<IActionResult> DeleteAccountAsync([FromForm] string username, [FromForm] string password, CancellationToken ct)
    {
        var claimsUsername = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value
            ?? throw new InvalidOperationException($"{nameof(ClaimTypes.Name)} claim could not be found in JWT token.");

        if(claimsUsername != username)
        {
            throw new UnauthorizedAccessException($"Not authorized to delete user: '{username}'");
        }

        await _identityService.DeleteUser(username, password);
        return Ok();
    }
}
