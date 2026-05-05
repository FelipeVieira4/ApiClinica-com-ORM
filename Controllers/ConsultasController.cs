using System.Text.RegularExpressions;
using ApiClinica.Data;
using ApiClinica.DTOs;
using ApiClinica.Mappers;
using ApiClinica.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiClinica.Controllers;

/**
 * Descrição: Consulta  Controller
 * Criado por: Felipe Vieira
 * Data: 04/05/2026
 */

[ApiController]
[Route("api/[controller]")]

public class ConsultasController: ControllerBase
{
    private readonly AppDbContext _context;

    public ConsultasController()
    {
        _context = new AppDbContext();
    }

    [HttpGet]
    public async Task<IActionResult> GetConsultas()
    {
        var consultas = await _context.Consultas.ToListAsync();

        var consultasReadDTO = consultas
            .Select(c => ConsultaMapper.ToReadDTO(c))
            .ToList();

        return Ok(consultasReadDTO);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetConsultaById(int id)
    {
        var consulta = await _context.Consultas.FindAsync(id);

        if (consulta == null) return NotFound();

        return Ok(ConsultaMapper.ToReadDTO(consulta));
    }

    [HttpPost]
    public async Task<IActionResult> CreateConsulta([FromBody] ConsultaCreateDTO consultaCreateDTO)
    {
        var consulta = ConsultaMapper.ToModel(consultaCreateDTO);

        if (consulta.DataHoraConsulta < DateTime.Now){
            return BadRequest(new { mensagem = "Data e hora da consulta não pode ser no passado!" });
        }

        var consultas = await _context.Consultas.ToListAsync();
        var consultasFiltradas = consultas
            .Where(c => c.DataHoraConsulta >= DateTime.Now)
            .Where(c => c.MedicoId == consulta.MedicoId || c.PacienteId == consulta.PacienteId)
            //.Where(c => c.PacienteId == consulta.PacienteId)
            .ToList();

        DateTime dataHora = (DateTime)consulta.DataHoraConsulta;
        foreach (var consulta2 in consultasFiltradas)
        {
            if (dataHora>=consulta2.DataHoraConsulta && dataHora<=consulta2.DataHoraConsulta.AddMinutes(30)||
                dataHora.AddMinutes(30)>=consulta2.DataHoraConsulta && dataHora.AddMinutes(30)<=consulta2.DataHoraConsulta.AddMinutes(30))
            {
                return BadRequest(new { mensagem = "Já existe uma consulta agendada para esse horário!" });
            }
        }

        var medico = await _context.Medicos.FindAsync(consulta.MedicoId);
        if (medico == null) return BadRequest(new { mensagem = "Médico não encontrado!" });

        var paciente = await _context.Pacientes.FindAsync(consulta.PacienteId);
        if (paciente == null) return BadRequest(new { mensagem = "Paciente não encontrado!" });

        _context.Consultas.Add(consulta);
        await _context.SaveChangesAsync();

        var consultaReadDTO = ConsultaMapper.ToReadDTO(consulta);

        return CreatedAtAction(nameof(GetConsultaById), new { id = consulta.Id }, consultaReadDTO);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateConsulta(int id, [FromBody] ConsultaUpdateDTO consultaUpdateDTO)
    {
        var consulta = await _context.Consultas.FindAsync(id);

        if(consulta == null) return NotFound();

        if (consultaUpdateDTO.PacienteId is not null)
        {
            var paciente = await _context.Pacientes.FindAsync(consulta.PacienteId);
            if (paciente == null) return BadRequest(new { mensagem = "Paciente não encontrado!" });
            
            consulta.PacienteId=(int)consultaUpdateDTO.PacienteId;
        }

        if (consultaUpdateDTO.MedicoId is not null)
        {
            var medico = await _context.Medicos.FindAsync(consulta.MedicoId);
            if (medico == null) return BadRequest(new { mensagem = "Médico não encontrado!" });

            consulta.MedicoId=(int)consultaUpdateDTO.MedicoId;
        }

        if (consultaUpdateDTO.DataHoraConsulta is not null)
        {
            if (consultaUpdateDTO.DataHoraConsulta < DateTime.Now)
            {
                return BadRequest(new { mensagem = "Data e hora da consulta não pode ser no passado!" });
            }

            consulta.DataHoraConsulta=(DateTime)consultaUpdateDTO.DataHoraConsulta;
        }

        var consultasLista = await _context.Consultas.ToListAsync();
        var consultasFiltradas = consultasLista
        .Where(c => c.DataHoraConsulta >= DateTime.Now)
        .Where(c => (c.MedicoId == consulta.MedicoId || c.PacienteId == consulta.PacienteId) && c.Id != consulta.Id)
        .ToList();

        DateTime dataHora = (DateTime)consulta.DataHoraConsulta;
        foreach (var consulta2 in consultasFiltradas)
        {
            if (dataHora>=consulta2.DataHoraConsulta && dataHora<=consulta2.DataHoraConsulta.AddMinutes(30)||
                dataHora.AddMinutes(30)>=consulta2.DataHoraConsulta && dataHora.AddMinutes(30)<=consulta2.DataHoraConsulta.AddMinutes(30))
            {
                return BadRequest(new { mensagem = "Já existe uma consulta agendada para esse horário!" });
            }
        }

        await _context.SaveChangesAsync();

        return Ok(ConsultaMapper.ToReadDTO(consulta));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteConsultas(int id)
    {
        var consulta = await _context.Consultas.FindAsync(id);

        if (consulta == null) return NotFound();

        _context.Consultas.Remove(consulta);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}