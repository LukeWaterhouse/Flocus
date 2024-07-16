using Flocus.Identity.Interfaces;
using Flocus.Identity.Models;
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
    private readonly IdentitySettings _identitySettings; //TODO: remove

    public IdentityController(ILogger<IdentityController> logger, IIdentityService identityService, IdentitySettings identitySettings)
    {
        _logger = logger;
        _identityService = identityService;
        _identitySettings = identitySettings;
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

        if (!ModelState.IsValid)
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

    [HttpPost("getToken", Name = "getToken")]
    public async Task<IActionResult> GetTokenAsync([FromForm] string username, [FromForm] string password, CancellationToken ct)
    {
        var token = await _identityService.GetAuthTokenAsync(username, password);
        return token != null ? Ok(token) : StatusCode(500, "Error generating token");
    }

    [Authorize]
    [HttpDelete("deleteUserAsUser", Name = "deleteUserAsUser")]
    public async Task<IActionResult> DeleteUserAsUserAsync([FromForm] string username, [FromForm] string password, CancellationToken ct)
    {
        var claimsUsername = GetClaimByType(ClaimTypes.Name);

        if (claimsUsername != username)
        {
            throw new UnauthorizedAccessException($"Not authorized to delete user: '{username}'");
        }

        await _identityService.DeleteUserAsUser(username, password);
        return Ok();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("deleteUserAsAdmin", Name = "deleteUserAsAdmin")]
    public async Task<IActionResult> DeleteUserAsAdminAsync([FromForm] string username, CancellationToken ct)
    {
        await _identityService.DeleteUserAsAdmin(username);
        return Ok();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("deleteAdmin", Name = "deleteAdmin")]
    public async Task<IActionResult> DeleteAdminAsync([FromForm] string username, [FromForm] string? password, [FromForm] string? key, CancellationToken ct)
    {
        var claimsUsername = GetClaimByType(ClaimTypes.Name);

        if (claimsUsername == username)
        {
            var notNullPassword = password ?? throw new UnauthorizedAccessException("You must provide a password when deleting your own admin account"); // check if you can throw here or must be formed response
            await _identityService.DeleteAdminAsAdmin(username, notNullPassword);
            return Ok();
        }

        var notNullAdminKey = key ?? throw new UnauthorizedAccessException($"You must provide an admin key when deleting another admin account: {username}");
        await _identityService.DeleteAdminAsAdminWithKey(username, notNullAdminKey);
        return Ok();
    }

    private string GetClaimByType(string claimType) // move this somewhere more global for user in user controller
    {
        return User.Claims.FirstOrDefault(c => c.Type == claimType)?.Value
            ?? throw new InvalidOperationException($"{claimType} claim could not be found in JWT token.");
    }
}
