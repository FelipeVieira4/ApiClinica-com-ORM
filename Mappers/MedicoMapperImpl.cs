using ApiClinica.Models;
using ApiClinica.DTOs;
using ApiClinica.Interfaces;

namespace ApiClinica.Mappers;

public class MedicoMapperImpl : IMedicoMapper
{
    public Medico ToModel(MedicoCreateDTO dto)
    {
        return new Medico
        {
            Nome = dto.Nome,
            Email = dto.Email,
            Telefone = dto.Telefone,
            CRM = dto.CRM
        };
    }

    public MedicoReadDTO ToDTO(Medico medico)
    {
        return new MedicoReadDTO
        {
            Id = medico.Id,
            Nome = medico.Nome,
            Email = medico.Email,
            Telefone = medico.Telefone,
            CRM = medico.CRM
        };
    }
}
