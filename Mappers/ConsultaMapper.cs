using ApiClinica.DTOs;
using ApiClinica.Models;

namespace ApiClinica.Mappers;

/**
 * Descrição: Consulta Mapper
 * Criado por: Felipe Vieira
 * Data: 03/05/2026
 */

public static class ConsultaMapper
{
    public static Consulta ToModel(ConsultaCreateDTO consultaCreateDTO)
    {
        return new Consulta
        {
            PacienteId = consultaCreateDTO.PacienteId,
            MedicoId = consultaCreateDTO.MedicoId,
            DataHoraConsulta = consultaCreateDTO.DataHoraConsulta
        };
    }

    public static ConsultaReadDTO ToReadDTO(Consulta consulta)
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