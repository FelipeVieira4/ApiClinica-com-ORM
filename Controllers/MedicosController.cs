using Microsoft.AspNetCore.Mvc;
using ApiClinica.Models;
using ApiClinica.Data;
using Microsoft.EntityFrameworkCore;
using ApiClinica.DTOs;
using ApiClinica.Mappers;
using Microsoft.Data.Sqlite;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;

namespace ApiClinica.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MedicosController : ControllerBase
{
    private readonly AppDbContext _context;
    private static readonly Regex TelefoneRegex = new("^\\d{10,11}$");

    public MedicosController()
    {
        _context = new AppDbContext();
    }

    // GET: api/medicos
    [HttpGet]
    public async Task<IActionResult> GetMedicos()
    {
        var medicos = await _context.Medicos.ToListAsync();

        var medicosDTO = medicos
        .Select(m => MedicoMapper.ToDTO(m))
        .ToList();

        return Ok(medicosDTO);
    }

    // GET: api/medicos/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult>
    GetMedicoById(int id)
    {
        var medico = await _context.Medicos.FindAsync(id);

        if(medico == null)
            return NotFound();

        return Ok(MedicoMapper.ToDTO(medico));
    }

    // POST: api/medicos
    [HttpPost]
    public async Task<IActionResult>
    CreateMedico([FromBody] MedicoCreateDTO dto)
    {
        if(!IsEmailValido(dto.Email))
        {
            return BadRequest(new { mensagem = "Email com formato invalido" });
        }

        if(!IsTelefoneValido(dto.Telefone))
        {
            return BadRequest(new { mensagem = "Telefone com formato invalido" });
        }

        var medico = MedicoMapper.ToModel(dto);

        _context.Medicos.Add(medico);
        await _context.SaveChangesAsync();

        var medicoDTO = MedicoMapper.ToDTO(medico);

        return CreatedAtAction(nameof(GetMedicoById), new {id = medico.Id}, medicoDTO);
    }

    // PATCH: api/medicos/{id}
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateMedico(int id, [FromBody] MedicoUpdateDTO dto)
    {
        var medico = await _context.Medicos.FindAsync(id);

        if(medico == null)
            return NotFound();

        if(dto.CRM is not null)
        {
            return BadRequest(new {mensagem = "CRM nao pode ser alterado no PATCH"});
        }

        if(dto.Email is not null && !IsEmailValido(dto.Email))
        {
            return BadRequest(new { mensagem = "Email com formato invalido." });
        }

        if(dto.Telefone is not null && !IsTelefoneValido(dto.Telefone))
        {
            return BadRequest(new {mensagem = "Telefone com formato invalido." });
        }

        if (dto.Nome is not null)
        {
            medico.Nome = dto.Nome;
        }

        if (dto.Email is not null)
        {
            medico.Email = dto.Email;
        }

        if (dto.Telefone is not null)
        {
            medico.Telefone = dto.Telefone;
        }

        await _context.SaveChangesAsync();

        return Ok(MedicoMapper.ToDTO(medico));
    }

    // DELETE api/medicos/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult>
    DeleteMedico(int id)
    {
        var medico = await _context.Medicos.FindAsync(id);

        if(medico == null)
            return NotFound();

        if(await MedicoPossuiConsultaFutura(id))
        {
            return BadRequest(new { mensagem = "Nao e possivel remover medico com consultas futuras."});
        }

        _context.Medicos.Remove(medico);
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

    private async Task<bool> MedicoPossuiConsultaFutura(int medicoId)
    {
        var connection = _context.Database.GetDbConnection();
        var abriuConexao = connection.State != ConnectionState.Open;

        if (abriuConexao)
        {
            await connection.OpenAsync();
        }

        try
        {
            if (!await TabelaConsultasExiste(connection))
            {
                return false;
            }

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(1) FROM Consultas WHERE MedicoId = $medicoId AND DataHoraConsulta > $agora";

            var parametroMedicoId = command.CreateParameter();
            parametroMedicoId.ParameterName = "$medicoId";
            parametroMedicoId.Value = medicoId;
            command.Parameters.Add(parametroMedicoId);

            var parametroAgora = command.CreateParameter();
            parametroAgora.ParameterName = "$agora";
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
            if (abriuConexao && connection.State == ConnectionState.Open)
            {
                await connection.CloseAsync();
            }
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
