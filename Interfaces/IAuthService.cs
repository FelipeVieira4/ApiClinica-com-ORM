using ApiClinica.DTOs;

namespace ApiClinica.Services;

public interface IAuthService
{
    Task<bool> RegisterAsync(RegisterDTO dto);
    Task<string?> AuthenticateAsync(LoginDTO dto);
}
