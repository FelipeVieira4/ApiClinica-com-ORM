using System.ComponentModel.DataAnnotations;

namespace ApiClinica.Models;

public class Cliente
{
    public int Id { get; set; }
    public required string Nome { get; set; }

    [EmailAddress(ErrorMessage = "Email invalido")]
    public required string Email { get; set; }

    public required string Telefone { get; set; }
}