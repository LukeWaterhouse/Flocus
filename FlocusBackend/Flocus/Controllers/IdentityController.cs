using AutoMapper;
using Flocus.Identity.Interfaces;
using Flocus.Identity.Interfaces.AuthTokenInterfaces;
using Flocus.Identity.Interfaces.RegisterInterfaces;
using Flocus.Identity.Interfaces.RemoveAccountInterfaces;
using Flocus.Identity.Models;
using Flocus.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flocus.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class IdentityController : ControllerBase
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
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequestDto request)
    {
        var registrationModel = _mapper.Map<RegistrationModel>(request);
        await _registrationService.RegisterAsync(registrationModel);
        return Ok();
    }

    [HttpPost("token", Name = "GetToken")]
    public async Task<IActionResult> GetTokenAsync([FromForm] string username, [FromForm] string password)
    {
        var token = await _authTokenService.GetAuthTokenAsync(username, password);
        return Ok(token);
    }

    [Authorize]
    [HttpDelete("user", Name = "DeleteSelfUser")]
    public async Task<IActionResult> DeleteSelfUserAsync([FromForm] string password)
    {
        var claims = _claimsService.GetClaimsFromUser(User);
        await _removeAccountService.DeleteSelfUserAsync(claims.Username, password);
        return Ok();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("admin/user/{username}", Name = "DeleteUserAsAdmin")]
    public async Task<IActionResult> DeleteUserAsAdminAsync(string username, [FromForm] string? key)
    {
        await _removeAccountService.DeleteUserByNameAsync(username, key);
        return Ok();
    }
}
