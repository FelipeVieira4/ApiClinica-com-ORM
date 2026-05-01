using ApiClinica.Models;
using ApiClinica.DTOs;

namespace ApiClinica.Mappers;

public static class MedicoMapper
{
    public static Medico ToModel(MedicoCreateDTO dto)
    {
        return new Medico
        {
            Nome = dto.Nome,
            Email = dto.Email,
            Telefone = dto.Telefone,
            CRM = dto.CRM
        };
    }

    public static MedicoReadDTO ToDTO(Medico medico)
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
