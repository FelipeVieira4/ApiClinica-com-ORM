using ApiClinica.DTOs;

namespace ApiClinica.Interfaces;

public interface IConsultaService
{
    Task<IEnumerable<ConsultaReadDTO>> GetAllAsync();
    Task<ConsultaReadDTO?> GetByIdAsync(int id);
    Task<ConsultaReadDTO> CreateAsync(ConsultaCreateDTO dto);
    Task<ConsultaReadDTO?> UpdateAsync(int id, ConsultaUpdateDTO dto);
    Task<bool> DeleteAsync(int id);
}
