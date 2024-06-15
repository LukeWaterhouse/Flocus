using Flocus.Identity.Interfaces;
using Flocus.Models.Errors;
using Flocus.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        if (!ModelState.IsValid) //TODO: consider moving this to a common function, also integration testing
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
    public async Task<IActionResult> DeleteUserAsync([FromForm] string username, [FromForm] string? password, CancellationToken ct)
    {
        var claimsUsername = GetClaimByType(ClaimTypes.Name);
        var claimsRole = GetClaimByType(ClaimTypes.Role);
        var adminRole = "Admin"; //TODO: make this from a provider

        if (claimsRole == adminRole)
        {
            await _identityService.DeleteUserAsAdmin(username);
            return Ok();
        }

        if (claimsUsername != username && claimsRole != adminRole)
        {
            throw new UnauthorizedAccessException($"Not authorized to delete user: '{username}'");
        }

        if(password == null)
        {
            throw new InvalidOperationException("must provide password if user");
        }

        await _identityService.DeleteUserAsUser(username, password);
        return Ok();
    }

    // can only delete other admins if you have key
    [Authorize]
    [HttpDelete("deleteAdmin", Name = "deleteAdmin")]
    public async Task<IActionResult> DeleteAdminAsync([FromForm] string username, [FromForm] string password, CancellationToken ct)
    {
        var claimsUsername = GetClaimByType(ClaimTypes.Name);
        var claimsRole = GetClaimByType(ClaimTypes.Role);
        var adminRole = "Admin"; //TODO: make this from a provider

        if (claimsUsername != username && claimsRole != adminRole)
        {
            throw new UnauthorizedAccessException($"Not authorized to delete user: '{username}'");
        }

        if (claimsRole == adminRole)
        {
            await _identityService.DeleteUserAsAdmin(username);
            return Ok();
        }

        await _identityService.DeleteUserAsUser(username, password);
        return Ok();
    }

    private string GetClaimByType(string claimType) // move this somewhere more global for user in user controller
    {
        return User.Claims.FirstOrDefault(c => c.Type == claimType)?.Value
            ?? throw new InvalidOperationException($"{claimType} claim could not be found in JWT token.");
    }
}
