using System.ComponentModel.DataAnnotations;

namespace ApiClinica.DTOs;

/**
 * Descrição: Consulta Update - Data Transfer Object(DTO)
 * Criado por: Felipe Vieira
 * Data: 03/05/2026
 */

public class ConsultaUpdateDTO
{
    public int? PacienteId { get; set; }
    public int? MedicoId { get; set; }
    public DateTime? DataHoraConsulta { get; set; }
}
