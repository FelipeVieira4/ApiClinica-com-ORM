using System.Text.RegularExpressions;
using ApiClinica.Data;
using ApiClinica.DTOs;
using ApiClinica.Mappers;
using ApiClinica.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiClinica.Interfaces;

namespace ApiClinica.Controllers;

[ApiController]
[Route("api/[controller]")]
[Microsoft.AspNetCore.Authorization.Authorize]
public class ConsultasController: ControllerBase
{
    private readonly IConsultaService _service;

    public ConsultasController(IConsultaService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetConsultas()
    {
        var consultas = await _service.GetAllAsync();
        return Ok(consultas);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetConsultaById(int id)
    {
        var consulta = await _service.GetByIdAsync(id);
        if (consulta == null) return NotFound();
        return Ok(consulta);
    }

    [HttpPost]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateConsulta([FromBody] ConsultaCreateDTO consultaCreateDTO)
    {
        try
        {
            var consulta = await _service.CreateAsync(consultaCreateDTO);
            return CreatedAtAction(nameof(GetConsultaById), new { id = consulta.Id }, consulta);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPatch("{id}")]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateConsulta(int id, [FromBody] ConsultaUpdateDTO consultaUpdateDTO)
    {
        try
        {
            var consulta = await _service.UpdateAsync(id, consultaUpdateDTO);
            if (consulta == null) return NotFound();
            return Ok(consulta);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConsultas(int id)
    {
        var ok = await _service.DeleteAsync(id);
        if (!ok) return NotFound();
        return NoContent();
    }
}