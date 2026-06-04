using ApiClinica.Interfaces;
using ApiClinica.DTOs;
using ApiClinica.Data;
using ApiClinica.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using System.Data.Common;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ApiClinica.Services;

public class MedicoService : IMedicoService
{
    private readonly AppDbContext _db;
    private readonly IMedicoMapper _mapper;
    private static readonly Regex TelefoneRegex = new("^\\d{10,11}$");

    public MedicoService(AppDbContext db, IMedicoMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<IEnumerable<MedicoReadDTO>> GetAllAsync()
    {
        var medicos = await _db.Medicos.ToListAsync();
        return medicos.Select(m => _mapper.ToDTO(m));
    }

    public async Task<MedicoReadDTO?> GetByIdAsync(int id)
    {
        var medico = await _db.Medicos.FindAsync(id);
        return medico == null ? null : _mapper.ToDTO(medico);
    }

    public async Task<MedicoReadDTO> CreateAsync(MedicoCreateDTO dto)
    {
        if (!IsEmailValido(dto.Email)) throw new ArgumentException("Email com formato inválido.");
        if (!IsTelefoneValido(dto.Telefone)) throw new ArgumentException("Telefone com formato inválido.");

        var medico = _mapper.ToModel(dto);
        _db.Medicos.Add(medico);
        await _db.SaveChangesAsync();
        return _mapper.ToDTO(medico);
    }

    public async Task<MedicoReadDTO?> UpdateAsync(int id, MedicoUpdateDTO dto)
    {
        var medico = await _db.Medicos.FindAsync(id);
        if (medico == null) return null;

        if (dto.CRM is not null) throw new ArgumentException("CRM nao pode ser alterado no PATCH");
        if (dto.Email is not null && !IsEmailValido(dto.Email)) throw new ArgumentException("Email com formato inválido.");
        if (dto.Telefone is not null && !IsTelefoneValido(dto.Telefone)) throw new ArgumentException("Telefone com formato inválido.");

        if (dto.Nome is not null) medico.Nome = dto.Nome;
        if (dto.Email is not null) medico.Email = dto.Email;
        if (dto.Telefone is not null) medico.Telefone = dto.Telefone;

        await _db.SaveChangesAsync();
        return _mapper.ToDTO(medico);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var medico = await _db.Medicos.FindAsync(id);
        if (medico == null) return false;

        if (await MedicoPossuiConsultaFutura(id)) throw new InvalidOperationException("Nao e possivel remover medico com consultas futuras.");

        _db.Medicos.Remove(medico);
        await _db.SaveChangesAsync();
        return true;
    }

    private static bool IsEmailValido(string email)
    {
        var emailValidator = new EmailAddressAttribute();
        return emailValidator.IsValid(email);
    }

    private static bool IsTelefoneValido(string telefone)
    {
        return TelefoneRegex.IsMatch(telefone);
    }

    private async Task<bool> MedicoPossuiConsultaFutura(int medicoId)
    {
        var connection = _db.Database.GetDbConnection();
        var abriuConexao = connection.State != System.Data.ConnectionState.Open;

        if (abriuConexao) await connection.OpenAsync();

        try
        {
            if (!await TabelaConsultasExiste(connection)) return false;

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(1) FROM Consultas WHERE MedicoId = @medicoId AND DataHoraConsulta >= @agora";

            var parametroMedicoId = command.CreateParameter();
            parametroMedicoId.ParameterName = "@medicoId";
            parametroMedicoId.Value = medicoId;
            command.Parameters.Add(parametroMedicoId);

            var parametroAgora = command.CreateParameter();
            parametroAgora.ParameterName = "@agora";
            parametroAgora.Value = DateTime.Now;
            command.Parameters.Add(parametroAgora);

            var resultado = await command.ExecuteScalarAsync();
            var quantidade = Convert.ToInt32(resultado);

            return quantidade > 0;
        }
        catch (SqliteException ex) when (ex.SqliteErrorCode == 1 && ex.Message.Contains("no such table", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }
        finally
        {
            if (abriuConexao && connection.State == System.Data.ConnectionState.Open) await connection.CloseAsync();
        }
    }

    private static async Task<bool> TabelaConsultasExiste(DbConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(1) FROM sqlite_master WHERE type = 'table' AND name = 'Consultas'";

        var resultado = await command.ExecuteScalarAsync();
        var quantidade = Convert.ToInt32(resultado);
        return quantidade > 0;
    }
}
