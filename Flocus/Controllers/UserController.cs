using AutoMapper;
using Flocus.Domain.Interfaces;
using Flocus.Identity.Interfaces;
using Flocus.Models.ReturnModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flocus.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _userService;
    private readonly IClaimsService _claimsService;
    private readonly IMapper _mapper;

    private readonly string AdminClaimsRole = "Admin";

    public UserController(ILogger<UserController> logger, IUserService userService, IClaimsService claimsService, IMapper mapper)
    {
        _logger = logger;
        _userService = userService;
        _claimsService = claimsService;
        _mapper = mapper;
    }

    [Authorize]
    [HttpGet(Name = "GetUser")]
    public async Task<IActionResult> GetUserAsync(string? username)
    {
        var claims = _claimsService.GetClaimsFromUser(User);
        var usernameToRetrieve = claims.Username;

        if (username != null && username != claims.Username)
        {
            if (claims.Role != AdminClaimsRole)
            {
                throw new UnauthorizedAccessException($"Must be '{AdminClaimsRole}' to access other users.");
            }
            usernameToRetrieve = username;
        }

        var user = await _userService.GetUserAsync(usernameToRetrieve);
        var userDto = _mapper.Map<UserDto>(user);

        return Ok(userDto);
    }
}
