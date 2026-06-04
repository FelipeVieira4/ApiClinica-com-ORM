using Microsoft.AspNetCore.Mvc;
using ApiClinica.DTOs;
using ApiClinica.Services;

namespace ApiClinica.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDTO dto)
    {
        var ok = await _authService.RegisterAsync(dto);
        if (!ok) return Conflict(new { message = "Username already exists" });
        return Created("", new { message = "User created" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDTO dto)
    {
        var token = await _authService.AuthenticateAsync(dto);
        if (token == null) return Unauthorized(new { message = "Invalid credentials" });
        return Ok(new { token });
    }
}
