using AutoMapper;
using Flocus.Domain.Interfaces;
using Flocus.Models.ReturnModels;
using Microsoft.AspNetCore.Mvc;

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

    [HttpGet("getUser", Name = "getUser")]
    public async Task<IActionResult> GetUserAsync(string username)
    {
        var user = await _userService.GetUserAsync(username);
        var userDto = _mapper.Map<UserDto>(user);

        return Ok(userDto);
    }
}
