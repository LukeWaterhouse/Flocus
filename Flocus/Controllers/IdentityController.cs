using AutoMapper;
using Flocus.Identity.Interfaces;
using Flocus.Identity.Interfaces.AuthTokenInterfaces;
using Flocus.Identity.Interfaces.RegisterInterfaces;
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
    private readonly IRemoveAccountService _removeAccountService;
    private readonly IRegistrationService _registrationService;
    private readonly IAuthTokenService _authTokenService;
    private readonly IClaimsService _claimsService;
    private readonly IMapper _mapper;

    public IdentityController(
        ILogger<IdentityController> logger,
        IMapper mapper,
        IClaimsService claimsService,
        IRegistrationService registrationService,
        IAuthTokenService authTokenService,
        IRemoveAccountService identityService)
    {
        _logger = logger;
        _removeAccountService = identityService;
        _registrationService = registrationService;
        _authTokenService = authTokenService;
        _claimsService = claimsService;
        _mapper = mapper;
    }

    [HttpPost("register", Name = "Register")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequestDto request, CancellationToken ct)
    {
        var registrationModel = _mapper.Map<RegistrationModel>(request);
        await _registrationService.RegisterAsync(registrationModel);
        return Ok();
    }

    [HttpPost("token", Name = "GetToken")]
    public async Task<IActionResult> GetTokenAsync([FromForm] string username, [FromForm] string password, CancellationToken ct)
    {
        var token = await _authTokenService.GetAuthTokenAsync(username, password);
        return Ok(token);
    }

    [Authorize]
    [HttpDelete("user", Name = "DeleteUserAsUser")]
    public async Task<IActionResult> DeleteUserAsUserAsync([FromForm] string password, CancellationToken ct)
    {
        var claims = _claimsService.GetClaimsFromUser(User);
        await _removeAccountService.DeleteUserAsUser(claims.Username, password);
        return Ok();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("userAsAdmin", Name = "DeleteUserAsAdmin")]
    public async Task<IActionResult> DeleteUserAsAdminAsync([FromForm] string username, CancellationToken ct)
    {
        await _removeAccountService.DeleteUserAsAdmin(username);
        return Ok();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("adminUser", Name = "DeleteAdmin")]
    public async Task<IActionResult> DeleteAdminAsync([FromForm] string username, [FromForm] string? password, [FromForm] string? key, CancellationToken ct)
    {
        var claims = _claimsService.GetClaimsFromUser(User);

        if (claims.Username == username)
        {
            var nullCheckedPassword = password ?? throw new UnauthorizedAccessException("You must provide a password when deleting your own admin account");
            await _removeAccountService.DeleteAdminAsAdmin(username, nullCheckedPassword);
            return Ok();
        }

        var nullCheckedAdminKey = key ?? throw new UnauthorizedAccessException($"You must provide an admin key when deleting another admin account: {username}");
        await _removeAccountService.DeleteAdminAsAdminWithKey(username, nullCheckedAdminKey);
        return Ok();
    }
}
