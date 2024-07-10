using AutoMapper;
using Flocus.Domain.Interfaces;
using Flocus.Models.ReturnModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Flocus.Controllers;

public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public UserController(ILogger<UserController> logger, IUserService userService, IMapper mapper)
    {
        _logger = logger;
        _userService = userService;
        _mapper = mapper;
    }

    [Authorize]
    [HttpGet("getUser", Name = "getUser")]
    public async Task<IActionResult> GetUserAsync()
    {
        var claimsUsername = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value
            ?? throw new InvalidOperationException($"{nameof(ClaimTypes.Name)} claim could not be found in JWT token.");

        var user = await _userService.GetUserAsync(claimsUsername);
        var userDto = _mapper.Map<UserDto>(user);

        return Ok(userDto);
    }
}
