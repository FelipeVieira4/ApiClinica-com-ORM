using ApiClinica.DTOs;
using ApiClinica.Models;

namespace ApiClinica.Mappers;

public static class ClienteMapper
{
    public static Cliente ToModel(ClienteCreateDTO dto)
    {
        return new Cliente
        {
            Nome = dto.Nome,
            Email = dto.Email,
            Telefone = dto.Telefone
        };
    }

    public static ClienteReadDTO ToDTO(Cliente cliente)
    {
        return new ClienteReadDTO
        {
            Id = cliente.Id,
            Nome = cliente.Nome,
            Email = cliente.Email,
            Telefone = cliente.Telefone
        };
    }
}