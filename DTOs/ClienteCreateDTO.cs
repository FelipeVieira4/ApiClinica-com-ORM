using System.ComponentModel.DataAnnotations;

namespace ApiClinica.DTOs;

public class ClienteCreateDTO
{
    public required string Nome { get; set; }

    [EmailAddress(ErrorMessage = "Email invalido")]
    public required string Email { get; set; }

    public required string Telefone { get; set; }
}