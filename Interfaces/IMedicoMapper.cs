using ApiClinica.Models;
using ApiClinica.DTOs;

namespace ApiClinica.Interfaces;

public interface IMedicoMapper
{
    Medico ToModel(MedicoCreateDTO dto);
    MedicoReadDTO ToDTO(Medico medico);
}
