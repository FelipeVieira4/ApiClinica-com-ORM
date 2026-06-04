using System.ComponentModel.DataAnnotations;

namespace ApiClinica.DTOs;

public class RegisterDTO
{
    [Required]
    public required string Username { get; set; }

    [Required]
    public required string Password { get; set; }

    // Optional: "User" or "Admin". Default is "User" if not provided.
    public string Role { get; set; } = "User";
}
