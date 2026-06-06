using System.ComponentModel.DataAnnotations;

namespace ApiClinica.DTOs;

public class RegisterDTO
{
    [Required]
    public required string Username { get; set; }

    [Required]
    public required string Password { get; set; }

    public string Role { get; set; } = "User";
}
