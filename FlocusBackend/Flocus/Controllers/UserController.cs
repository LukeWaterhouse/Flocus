using AutoMapper;
using Flocus.Domain.Interfaces;
using Flocus.Identity.Interfaces;
using Flocus.Models.ReturnModels.UserReturnModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flocus.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _userService;
    private readonly IClaimsService _claimsService;
    private readonly IMapper _mapper;

    public UserController(ILogger<UserController> logger, IUserService userService, IClaimsService claimsService, IMapper mapper)
    {
        _logger = logger;
        _userService = userService;
        _claimsService = claimsService;
        _mapper = mapper;
    }

    [Authorize]
    [HttpGet(Name = "GetUser")]
    public async Task<IActionResult> GetUserAsync()
    {
        var claims = _claimsService.GetClaimsFromUser(User);

        var user = await _userService.GetUserAsync(claims.Username);
        var userDto = _mapper.Map<UserSensitiveInfoDto>(user);

        return Ok(userDto);
    }


    // TODO: at some point could make this return list of users filterable by username so admin can get all users.
    [Authorize]
    [Authorize(Roles = "Admin")]
    [HttpGet("admin/user/{username}", Name = "GetUserAsAdmin")]
    public async Task<IActionResult> GetUserAsAdminAsync(string username)
    {
        var user = await _userService.GetUserAsync(username);
        var userDto = _mapper.Map<UserSensitiveInfoDto>(user);

        return Ok(userDto);
    }
}
