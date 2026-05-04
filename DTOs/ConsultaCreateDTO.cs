using System.ComponentModel.DataAnnotations;

namespace ApiClinica.DTOs;

/**
 * Descrição: Consulta Create - Data Transfer Object(DTO)
 * Criado por: Felipe Vieira
 * Data: 03/05/2026
 */

public class ConsultaCreateDTO
{
    public required int PacienteId { get; set; }
    public required int MedicoId { get; set; }

    public required DateTime DataHoraConsulta { get; set; }
}
