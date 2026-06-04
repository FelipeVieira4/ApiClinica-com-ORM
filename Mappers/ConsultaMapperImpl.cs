using ApiClinica.DTOs;
using ApiClinica.Models;
using ApiClinica.Interfaces;

namespace ApiClinica.Mappers;

public class ConsultaMapperImpl : IConsultaMapper
{
    public Consulta ToModel(ConsultaCreateDTO dto)
    {
        return new Consulta
        {
            PacienteId = dto.PacienteId,
            MedicoId = dto.MedicoId,
            DataHoraConsulta = dto.DataHoraConsulta
        };
    }

    public ConsultaReadDTO ToDTO(Consulta consulta)
    {
        return new ConsultaReadDTO
        {
            Id = consulta.Id,
            PacienteId = consulta.PacienteId,
            MedicoId = consulta.MedicoId,
            DataHoraConsulta = consulta.DataHoraConsulta
        };
    }
}
