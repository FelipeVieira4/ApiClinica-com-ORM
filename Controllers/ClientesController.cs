using ApiClinica.Data;
using ApiClinica.DTOs;
using ApiClinica.Mappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiClinica.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly AppDbContext _context;

    public ClientesController()
    {
        _context = new AppDbContext();
    }

    [HttpGet]
    public async Task<IActionResult> GetClientes()
    {
        var clientes = await _context.Clientes.ToListAsync();

        var clientesDTO = clientes
            .Select(c => ClienteMapper.ToDTO(c))
            .ToList();

        return Ok(clientesDTO);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetClienteById(int id)
    {
        var cliente = await _context.Clientes.FindAsync(id);

        if (cliente == null)
            return NotFound();

        return Ok(ClienteMapper.ToDTO(cliente));
    }

    [HttpPost]
    public async Task<IActionResult> CreateCliente([FromBody] ClienteCreateDTO dto)
    {
        var cliente = ClienteMapper.ToModel(dto);

        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();

        var clienteDTO = ClienteMapper.ToDTO(cliente);

        return CreatedAtAction(nameof(GetClienteById), new { id = cliente.Id }, clienteDTO);
    }
}