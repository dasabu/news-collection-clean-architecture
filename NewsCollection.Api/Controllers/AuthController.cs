using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsCollection.Application.Dtos;
using NewsCollection.Application.Interfaces;

namespace NewsCollection.Api.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterRequestDto request)
    {
        var user = await authService.RegisterAsync(request);

        if (user == null)
        {
            return BadRequest("Email already exists!");
        }
        return Ok(new UserDto(user.Id, user.Email, user.Name));
    }

    [HttpPost("login")]
    public async Task<ActionResult<TokenResponseDto>> Login(LoginRequestDto request)
    {
        var token = await authService.LoginAsync(request);
        return token == null ? BadRequest("Invalid username or password") : Ok(token);
    }
}