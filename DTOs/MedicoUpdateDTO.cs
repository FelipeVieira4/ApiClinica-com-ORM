using System.ComponentModel.DataAnnotations;

namespace ApiClinica.DTOs;

public class MedicoUpdateDTO
{
    public string? Nome { get; set; }

    [EmailAddress(ErrorMessage = "Email invalido")]
    public string? Email { get; set; }

    public string? Telefone { get; set; }
    public string? CRM { get; set; }
}
