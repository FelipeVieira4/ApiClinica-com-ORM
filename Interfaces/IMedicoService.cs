using ApiClinica.DTOs;

namespace ApiClinica.Interfaces;

public interface IMedicoService
{
    Task<IEnumerable<MedicoReadDTO>> GetAllAsync();
    Task<MedicoReadDTO?> GetByIdAsync(int id);
    Task<MedicoReadDTO> CreateAsync(MedicoCreateDTO dto);
    Task<MedicoReadDTO?> UpdateAsync(int id, MedicoUpdateDTO dto);
    Task<bool> DeleteAsync(int id);
}
