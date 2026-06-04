using System.ComponentModel.DataAnnotations;

namespace ApiClinica.Models;

public class Consulta
{
    public int Id { get; set; }

    public required int PacienteId { get; set; }
    public virtual Paciente? Paciente { get; set; }

    public required int MedicoId { get; set; }
    public virtual Medico? Medico { get; set; }

    public required DateTime DataHoraConsulta { get; set; }  // hórario da Consulta foi fixado em 30 min
}