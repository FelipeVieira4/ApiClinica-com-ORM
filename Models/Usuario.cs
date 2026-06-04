using System.ComponentModel.DataAnnotations;

namespace ApiClinica.Models;

public class Usuario
{
    [Key]
    public int Id { get; set; }

    [Required]
    public required string Username { get; set; }

    [Required]
    public required string PasswordHash { get; set; }

    [Required]
    public string Role { get; set; } = "User"; // "User" or "Admin"
}
