using ApiClinica.DTOs;
using ApiClinica.Models;
using ApiClinica.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ApiClinica.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;

    public AuthService(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    public async Task<bool> RegisterAsync(RegisterDTO dto)
    {
        if (_db.Usuarios.Any(u => u.Username == dto.Username))
            return false;

        var user = new Usuario
        {
            Username = dto.Username,
            PasswordHash = ComputeHash(dto.Password),
            Role = string.IsNullOrWhiteSpace(dto.Role) ? "User" : dto.Role
        };

        _db.Usuarios.Add(user);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<string?> AuthenticateAsync(LoginDTO dto)
    {
        var user = _db.Usuarios.SingleOrDefault(u => u.Username == dto.Username);
        if (user == null) return null;
        if (user.PasswordHash != ComputeHash(dto.Password)) return null;

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? "change_this_secret");
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(double.TryParse(_config["Jwt:ExpireMinutes"], out var m) ? m : 60),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = _config["Jwt:Issuer"],
            Audience = _config["Jwt:Audience"]
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private static string ComputeHash(string input)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes);
    }
}
