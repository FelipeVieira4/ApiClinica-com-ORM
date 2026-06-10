using Microsoft.AspNetCore.Mvc;
using ApiClinica.DTOs;
using ApiClinica.Interfaces;

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
        if (!ok) return Conflict(new { message = "Usuário já existe" });
        return Created("", new { message = "Usuário criado" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDTO dto)
    {
        var token = await _authService.AuthenticateAsync(dto);
        if (token == null) return Unauthorized(new { message = "Credenciais inválidas" });
        return Ok(new { token });
    }
}
