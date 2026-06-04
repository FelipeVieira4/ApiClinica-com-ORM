namespace ApiClinica.DTOs;

public class ConsultaReadDTO
{
    public int Id { get; set; }

    public int PacienteId { get; set; } = default!;

    public int MedicoId { get; set; } = default!;

    public DateTime DataHoraConsulta { get; set; } = default!;
}
