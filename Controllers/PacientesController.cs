using Microsoft.AspNetCore.Mvc;
using ApiClinica.Models;
using ApiClinica.Data;
using Microsoft.EntityFrameworkCore;
using ApiClinica.DTOs;
using ApiClinica.Mappers;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text.RegularExpressions;

namespace ApiClinica.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PacientesController : ControllerBase
{
    private readonly AppDbContext _context;
    private static readonly Regex TelefoneRegex = new("^\\d{10,11}$");

    public PacientesController()
    {
        _context = new AppDbContext();
    }

    // GET: api/pacientes
    [HttpGet]
    public async Task<IActionResult> GetPacientes()
    {
        var pacientes = await _context.Pacientes.ToListAsync();

        var pacientesDTO = pacientes
            .Select(p => PacienteMapper.ToDTO(p))
            .ToList();

        return Ok(pacientesDTO);
    }

    // GET: api/pacientes/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPacienteById(int id)
    {
        var paciente = await _context.Pacientes.FindAsync(id);

        if (paciente == null)
            return NotFound();

        return Ok(PacienteMapper.ToDTO(paciente));
    }

    // POST: api/pacientes
    [HttpPost]
    public async Task<IActionResult> CreatePacient([FromBody] PacienteCreateDTO dto)
    {
        if (!IsEmailValido(dto.Email))
        {
            return BadRequest(new { mensagem = "Email com formato inválido." });
        }

        if (!IsTelefoneValido(dto.Telefone))
        {
            return BadRequest(new { mensagem = "Telefone com formato inválido." });
        }

        if (!IsCpfValido(dto.Cpf))
        {
            return BadRequest(new { mensagem = "CPF inválido." });
        }

        if (dto.DataNasc > DateOnly.FromDateTime(DateTime.Today))
        {
            return BadRequest(new { mensagem = "Data de nascimento não pode ser futura." });
        }

        var cpfJaExiste = await _context.Pacientes.AnyAsync(p => p.Cpf == dto.Cpf);

        if (cpfJaExiste)
        {
            return BadRequest(new { mensagem = "Já existe um paciente com este CPF." });
        }

        var paciente = PacienteMapper.ToModel(dto);

        _context.Pacientes.Add(paciente);
        await _context.SaveChangesAsync();

        var pacienteDTO = PacienteMapper.ToDTO(paciente);

        return CreatedAtAction(nameof(GetPacienteById), new { id = paciente.Id }, pacienteDTO);
    }

    // PATCH: api/pacientes/{id}
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdatePaciente(int id, [FromBody] PacienteUpdateDTO dto)
    {
        var paciente = await _context.Pacientes.FindAsync(id);

        if (paciente == null)
            return NotFound();

        if (!string.IsNullOrWhiteSpace(dto.Cpf))
        {
            return BadRequest(new { mensagem = "CPF não pode ser alterado no PATCH." });
        }

        if (dto.Email is not null && !IsEmailValido(dto.Email))
        {
            return BadRequest(new { mensagem = "Email com formato inválido." });
        }

        if (dto.Telefone is not null && !IsTelefoneValido(dto.Telefone))
        {
            return BadRequest(new { mensagem = "Telefone com formato inválido." });
        }

        if (dto.DataNasc is not null && dto.DataNasc > DateOnly.FromDateTime(DateTime.Today))
        {
            return BadRequest(new { mensagem = "Data de nascimento não pode ser futura." });
        }

        if (dto.Nome is not null)
        {
            paciente.Nome = dto.Nome;
        }

        if (dto.Email is not null)
        {
            paciente.Email = dto.Email;
        }

        if (dto.Telefone is not null)
        {
            paciente.Telefone = dto.Telefone;
        }

        if (dto.DataNasc is not null)
        {
            paciente.DataNasc = dto.DataNasc.Value;
        }

        await _context.SaveChangesAsync();

        return Ok(PacienteMapper.ToDTO(paciente));
    }

    // DELETE: api/pacientes/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePaciente(int id)
    {
        var paciente = await _context.Pacientes.FindAsync(id);

        if (paciente == null)
            return NotFound();

        if (await PacientePossuiConsultaFutura(id))
        {
            return BadRequest(new { mensagem = "Não é possível remover paciente com consultas futuras." });
        }

        _context.Pacientes.Remove(paciente);
        await _context.SaveChangesAsync();

        return NoContent();
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
        var cpfNumeros = new string(cpf.Where(char.IsDigit).ToArray());

        if (cpfNumeros.Length != 11)
            return false;

        if (cpfNumeros.Distinct().Count() == 1)
            return false;

        var somaPrimeiroDigito = 0;
        for (var i = 0; i < 9; i++)
        {
            somaPrimeiroDigito += (cpfNumeros[i] - '0') * (10 - i);
        }

        var restoPrimeiroDigito = somaPrimeiroDigito % 11;
        var primeiroDigitoCalculado = restoPrimeiroDigito < 2 ? 0 : 11 - restoPrimeiroDigito;

        if (cpfNumeros[9] - '0' != primeiroDigitoCalculado)
            return false;

        var somaSegundoDigito = 0;
        for (var i = 0; i < 10; i++)
        {
            somaSegundoDigito += (cpfNumeros[i] - '0') * (11 - i);
        }

        var restoSegundoDigito = somaSegundoDigito % 11;
        var segundoDigitoCalculado = restoSegundoDigito < 2 ? 0 : 11 - restoSegundoDigito;

        return cpfNumeros[10] - '0' == segundoDigitoCalculado;
    }

    private async Task<bool> PacientePossuiConsultaFutura(int pacienteId)
    {
        try
        {
            var connection = _context.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(1) FROM Consultas WHERE PacienteId = $pacienteId AND DataHora > $agora";

            var parametroPacienteId = command.CreateParameter();
            parametroPacienteId.ParameterName = "$pacienteId";
            parametroPacienteId.Value = pacienteId;
            command.Parameters.Add(parametroPacienteId);

            var parametroAgora = command.CreateParameter();
            parametroAgora.ParameterName = "$agora";
            parametroAgora.Value = DateTime.Now;
            command.Parameters.Add(parametroAgora);

            var resultado = await command.ExecuteScalarAsync();
            var quantidade = Convert.ToInt32(resultado);

            return quantidade > 0;
        }
        catch
        {
            return false;
        }
    }
}