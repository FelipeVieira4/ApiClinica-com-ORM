using ApiClinica.Models;
using ApiClinica.DTOs;

namespace ApiClinica.Interfaces;

public interface IConsultaMapper
{
    Consulta ToModel(ConsultaCreateDTO dto);
    ConsultaReadDTO ToDTO(Consulta consulta);
}
