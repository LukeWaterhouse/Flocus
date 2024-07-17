using AutoMapper;
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
    private readonly IMapper _mapper;


    public IdentityController(ILogger<IdentityController> logger, IMapper mapper, IIdentityService identityService)
    {
        _logger = logger;
        _identityService = identityService;
        _mapper = mapper;
    }

    [HttpPost("register", Name = "Register")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequestDto request, CancellationToken ct)
    {
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

        var registrationModel = _mapper.Map<RegistrationModel>(request);
        await _identityService.RegisterAsync(registrationModel);
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
