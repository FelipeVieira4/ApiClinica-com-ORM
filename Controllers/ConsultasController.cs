using System.Text.RegularExpressions;
using ApiClinica.Data;
using ApiClinica.DTOs;
using ApiClinica.Mappers;
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

public class ConsultaController: ControllerBase
{
    private readonly AppDbContext _context;

    public ConsultaController()
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

        if (consulta == null)
            return NotFound();

        return Ok(ConsultaMapper.ToReadDTO(consulta));
    }

    [HttpPost]
    public async Task<IActionResult> CreateConsulta([FromBody] ConsultaCreateDTO consultaCreateDTO)
    {
        var consulta = ConsultaMapper.ToModel(consultaCreateDTO);

        _context.Consultas.Add(consulta);
        await _context.SaveChangesAsync();

        var consultaReadDTO = ConsultaMapper.ToReadDTO(consulta);

        return CreatedAtAction(nameof(GetConsultaById), new { id = consulta.Id }, consultaReadDTO);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateConsulta(int id, [FromBody] ConsultaUpdateDTO consultaUpdateDTO)
    {
        var consulta = await _context.Consultas.FindAsync(id);

        if(consulta == null)
            return NotFound();

        if (consultaUpdateDTO.PacienteId is not null)
        {
            consulta.PacienteId=(int)consultaUpdateDTO.PacienteId;
        }

        if (consultaUpdateDTO.MedicoId is not null)
        {
            consulta.MedicoId=(int)consultaUpdateDTO.MedicoId;
        }

        if (consultaUpdateDTO.DataHoraConsulta is not null)
        {
            consulta.DataHoraConsulta=(DateTime)consultaUpdateDTO.DataHoraConsulta;
        }

        

        await _context.SaveChangesAsync();

        return Ok(ConsultaMapper.ToReadDTO(consulta));
    }

}