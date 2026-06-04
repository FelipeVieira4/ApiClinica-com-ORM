using ApiClinica.Interfaces;
using ApiClinica.DTOs;
using ApiClinica.Data;
using ApiClinica.Models;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ApiClinica.Services;

public class PacienteService : IPacienteService
{
    private readonly AppDbContext _db;
    private readonly IPacienteMapper _mapper;
    private static readonly Regex TelefoneRegex = new("^\\d{10,11}$");

    public PacienteService(AppDbContext db, IPacienteMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PacienteReadDTO>> GetAllAsync()
    {
        var pacientes = await _db.Pacientes.ToListAsync();
        return pacientes.Select(p => _mapper.ToDTO(p));
    }

    public async Task<PacienteReadDTO?> GetByIdAsync(int id)
    {
        var paciente = await _db.Pacientes.FindAsync(id);
        return paciente == null ? null : _mapper.ToDTO(paciente);
    }

    public async Task<PacienteReadDTO> CreateAsync(PacienteCreateDTO dto)
    {
        var cpfNormalizado = NormalizarCpf(dto.Cpf);

        if (!IsEmailValido(dto.Email)) throw new ArgumentException("Email com formato inválido.");
        if (!IsTelefoneValido(dto.Telefone)) throw new ArgumentException("Telefone com formato inválido.");
        if (!IsCpfValido(cpfNormalizado)) throw new ArgumentException("CPF inválido.");
        if (dto.DataNasc > DateOnly.FromDateTime(DateTime.Today)) throw new ArgumentException("Data de nascimento não pode ser futura.");

        var cpfsExistentes = await _db.Pacientes.AsNoTracking().Select(p => p.Cpf).ToListAsync();
        var cpfJaExiste = cpfsExistentes.Any(cpf => NormalizarCpf(cpf) == cpfNormalizado);
        if (cpfJaExiste) throw new ArgumentException("Já existe um paciente com este CPF.");

        var paciente = _mapper.ToModel(dto);
        paciente.Cpf = cpfNormalizado;

        _db.Pacientes.Add(paciente);
        await _db.SaveChangesAsync();

        return _mapper.ToDTO(paciente);
    }

    public async Task<PacienteReadDTO?> UpdateAsync(int id, PacienteUpdateDTO dto)
    {
        var paciente = await _db.Pacientes.FindAsync(id);
        if (paciente == null) return null;

        if (dto.Cpf is not null) throw new ArgumentException("CPF não pode ser alterado no PATCH.");
        if (dto.Email is not null && !IsEmailValido(dto.Email)) throw new ArgumentException("Email com formato inválido.");
        if (dto.Telefone is not null && !IsTelefoneValido(dto.Telefone)) throw new ArgumentException("Telefone com formato inválido.");
        if (dto.DataNasc is not null && dto.DataNasc > DateOnly.FromDateTime(DateTime.Today)) throw new ArgumentException("Data de nascimento não pode ser futura.");

        if (dto.Nome is not null) paciente.Nome = dto.Nome;
        if (dto.Email is not null) paciente.Email = dto.Email;
        if (dto.Telefone is not null) paciente.Telefone = dto.Telefone;
        if (dto.DataNasc is not null) paciente.DataNasc = dto.DataNasc.Value;

        await _db.SaveChangesAsync();
        return _mapper.ToDTO(paciente);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var paciente = await _db.Pacientes.FindAsync(id);
        if (paciente == null) return false;

        if (await PacientePossuiConsultaFutura(id)) throw new InvalidOperationException("Não é possível remover paciente com consultas futuras.");

        _db.Pacientes.Remove(paciente);
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

    private static bool IsCpfValido(string cpf)
    {
        var cpfNumeros = NormalizarCpf(cpf);

        if (cpfNumeros.Length != 11) return false;
        if (cpfNumeros.Distinct().Count() == 1) return false;

        var somaPrimeiroDigito = 0;
        for (var i = 0; i < 9; i++) somaPrimeiroDigito += (cpfNumeros[i] - '0') * (10 - i);
        var restoPrimeiroDigito = somaPrimeiroDigito % 11;
        var primeiroDigitoCalculado = restoPrimeiroDigito < 2 ? 0 : 11 - restoPrimeiroDigito;
        if (cpfNumeros[9] - '0' != primeiroDigitoCalculado) return false;

        var somaSegundoDigito = 0;
        for (var i = 0; i < 10; i++) somaSegundoDigito += (cpfNumeros[i] - '0') * (11 - i);
        var restoSegundoDigito = somaSegundoDigito % 11;
        var segundoDigitoCalculado = restoSegundoDigito < 2 ? 0 : 11 - restoSegundoDigito;
        return cpfNumeros[10] - '0' == segundoDigitoCalculado;
    }

    private static string NormalizarCpf(string cpf)
    {
        return new string(cpf.Where(char.IsDigit).ToArray());
    }

    private async Task<bool> PacientePossuiConsultaFutura(int pacienteId)
    {
        var connection = _db.Database.GetDbConnection();
        var abriuConexao = connection.State != System.Data.ConnectionState.Open;

        if (abriuConexao) await connection.OpenAsync();

        try
        {
            if (!await TabelaConsultasExiste(connection)) return false;

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(1) FROM Consultas WHERE PacienteId = @pacienteId AND DataHoraConsulta >= @agora";

            var parametroPacienteId = command.CreateParameter();
            parametroPacienteId.ParameterName = "@pacienteId";
            parametroPacienteId.Value = pacienteId;
            command.Parameters.Add(parametroPacienteId);

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
