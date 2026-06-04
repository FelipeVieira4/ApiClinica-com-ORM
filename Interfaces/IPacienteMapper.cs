using ApiClinica.Models;
using ApiClinica.DTOs;

namespace ApiClinica.Interfaces;

public interface IPacienteMapper
{
    Paciente ToModel(PacienteCreateDTO dto);
    PacienteReadDTO ToDTO(Paciente paciente);
}
