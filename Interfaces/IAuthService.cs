using ApiClinica.DTOs;

namespace ApiClinica.Interfaces;

public interface IAuthService
{
    Task<bool> RegisterAsync(RegisterDTO dto);
    Task<string?> AuthenticateAsync(LoginDTO dto);
}
