using System.ComponentModel.DataAnnotations;

namespace ApiClinica.Models;

/**
 * Descrição: Consulta Controller
 * Criado por: Felipe Vieira
 * Data: 03/05/2026
 */

public class Consulta
{
    public int Id { get; set; }

    public int PacienteId { get; set; }
    public virtual Paciente? Paciente { get; set; }

    public int MedicoId { get; set; }
    public virtual Medico? Medico { get; set; }

    public DateTime DataHoraConsulta { get; set; }  // hórario da Consulta foi fixado em 30 min
}