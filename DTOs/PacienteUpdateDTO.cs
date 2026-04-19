using System.ComponentModel.DataAnnotations;

namespace ApiClinica.DTOs;

public class PacienteUpdateDTO
{
    public string? Nome { get; set; }

    [EmailAddress(ErrorMessage = "Email inválido")]
    public string? Email { get; set; }

    public string? Telefone { get; set; }
    public DateOnly? DataNasc { get; set; }
    public string? Cpf { get; set; }
}