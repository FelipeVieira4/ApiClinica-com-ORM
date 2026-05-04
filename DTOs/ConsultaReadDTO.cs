namespace ApiClinica.DTOs;

/**
 * Descrição: Consulta Read - Data Transfer Object(DTO)
 * Criado por: Felipe Vieira
 * Data: 03/05/2026
 */

public class ConsultaReadDTO
{
    public int Id { get; set; }
    public int PacienteId { get; set; } = default!;
    public int MedicoId { get; set; } = default!;
    public DateTime DataHoraConsulta { get; set; } = default!;
}
