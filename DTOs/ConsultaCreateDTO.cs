using System.ComponentModel.DataAnnotations;

namespace ApiClinica.DTOs;

public class ConsultaCreateDTO
{
    [Required]
    public required int PacienteId { get; set; }

    [Required]
    public required int MedicoId { get; set; }

    [Required]
    public required DateTime DataHoraConsulta { get; set; }
}
