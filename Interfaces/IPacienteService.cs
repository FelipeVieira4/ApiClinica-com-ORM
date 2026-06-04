using ApiClinica.DTOs;

namespace ApiClinica.Interfaces;

public interface IPacienteService
{
    Task<IEnumerable<PacienteReadDTO>> GetAllAsync();
    Task<PacienteReadDTO?> GetByIdAsync(int id);
    Task<PacienteReadDTO> CreateAsync(PacienteCreateDTO dto);
    Task<PacienteReadDTO?> UpdateAsync(int id, PacienteUpdateDTO dto);
    Task<bool> DeleteAsync(int id);
}
