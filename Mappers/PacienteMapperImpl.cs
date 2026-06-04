using ApiClinica.Models;
using ApiClinica.DTOs;
using ApiClinica.Interfaces;

namespace ApiClinica.Mappers;

public class PacienteMapperImpl : IPacienteMapper
{
    public Paciente ToModel(PacienteCreateDTO dto)
    {
        return new Paciente
        {
            Nome = dto.Nome,
            Email = dto.Email,
            Telefone = dto.Telefone,
            DataNasc = dto.DataNasc,
            Cpf = dto.Cpf
        };
    }

    public PacienteReadDTO ToDTO(Paciente paciente)
    {
        return new PacienteReadDTO
        {
            Id = paciente.Id,
            Nome = paciente.Nome,
            Email = paciente.Email,
            Telefone = paciente.Telefone,
            DataNasc = paciente.DataNasc
        };
    }
}
