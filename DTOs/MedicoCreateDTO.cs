using System.ComponentModel.DataAnnotations;

namespace ApiClinica.DTOs;

public class MedicoCreateDTO
{
    public required string Nome { get; set; }

    [EmailAddress(ErrorMessage = "Email invalido")]
    public required string Email { get; set; }

    public required string Telefone { get; set; }

    public required string CRM { get; set; }
}
