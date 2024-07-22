using AutoMapper;
using Flocus.Identity.Interfaces;
using Flocus.Identity.Models;
using Flocus.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flocus.Controllers;

[ApiController]
[Route("[controller]")]
public class IdentityController : ControllerBase
{
    private readonly ILogger<IdentityController> _logger;
    private readonly IIdentityService _identityService;
    private readonly IClaimsService _claimsService;
    private readonly IMapper _mapper;

    public IdentityController(ILogger<IdentityController> logger, IMapper mapper, IClaimsService claimsService, IIdentityService identityService)
    {
        _logger = logger;
        _identityService = identityService;
        _claimsService = claimsService;
        _mapper = mapper;
    }

    [HttpPost("register", Name = "Register")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequestDto request, CancellationToken ct)
    {
        var registrationModel = _mapper.Map<RegistrationModel>(request);
        await _identityService.RegisterAsync(registrationModel);
        return Ok();
    }

    [HttpPost("getToken", Name = "GetToken")]
    public async Task<IActionResult> GetTokenAsync([FromForm] string username, [FromForm] string password, CancellationToken ct)
    {
        var token = await _identityService.GetAuthTokenAsync(username, password);
        return token != null ? Ok(token) : StatusCode(500, "Error generating token");
    }

    [Authorize]
    [HttpDelete("deleteUserAsUser", Name = "DeleteUserAsUser")]
    public async Task<IActionResult> DeleteUserAsUserAsync([FromForm] string username, [FromForm] string password, CancellationToken ct)
    {
        var claims = _claimsService.GetClaimsFromUser(User);

        if (claims.Username != username)
        {
            throw new UnauthorizedAccessException($"Not authorized to delete user: '{username}'");
        }

        await _identityService.DeleteUserAsUser(username, password);
        return Ok();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("deleteUserAsAdmin", Name = "DeleteUserAsAdmin")]
    public async Task<IActionResult> DeleteUserAsAdminAsync([FromForm] string username, CancellationToken ct)
    {
        await _identityService.DeleteUserAsAdmin(username);
        return Ok();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("deleteAdmin", Name = "DeleteAdmin")]
    public async Task<IActionResult> DeleteAdminAsync([FromForm] string username, [FromForm] string? password, [FromForm] string? key, CancellationToken ct)
    {
        var claims = _claimsService.GetClaimsFromUser(User);

        if (claims.Username == username)
        {
            var notNullPassword = password ?? throw new UnauthorizedAccessException("You must provide a password when deleting your own admin account");
            await _identityService.DeleteAdminAsAdmin(username, notNullPassword);
            return Ok();
        }

        var notNullAdminKey = key ?? throw new UnauthorizedAccessException($"You must provide an admin key when deleting another admin account: {username}");
        await _identityService.DeleteAdminAsAdminWithKey(username, notNullAdminKey);
        return Ok();
    }
}
