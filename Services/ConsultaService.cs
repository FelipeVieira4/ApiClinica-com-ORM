using ApiClinica.Interfaces;
using ApiClinica.DTOs;
using ApiClinica.Data;
using ApiClinica.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiClinica.Services;

public class ConsultaService : IConsultaService
{
    private readonly AppDbContext _db;
    private readonly IConsultaMapper _mapper;

    public ConsultaService(AppDbContext db, IConsultaMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ConsultaReadDTO>> GetAllAsync()
    {
        var consultas = await _db.Consultas.ToListAsync();
        return consultas.Select(c => _mapper.ToDTO(c));
    }

    public async Task<ConsultaReadDTO?> GetByIdAsync(int id)
    {
        var consulta = await _db.Consultas.FindAsync(id);
        return consulta == null ? null : _mapper.ToDTO(consulta);
    }

    public async Task<ConsultaReadDTO> CreateAsync(ConsultaCreateDTO dto)
    {
        var consulta = _mapper.ToModel(dto);

        if (consulta.DataHoraConsulta < DateTime.Now) throw new ArgumentException("Data e hora da consulta não pode ser no passado!");

        var consultas = await _db.Consultas.ToListAsync();
        var consultasFiltradas = consultas
            .Where(c => c.DataHoraConsulta >= DateTime.Now)
            .Where(c => c.MedicoId == consulta.MedicoId || c.PacienteId == consulta.PacienteId)
            .ToList();

        DateTime dataHora = (DateTime)consulta.DataHoraConsulta;
        foreach (var consulta2 in consultasFiltradas)
        {
            if (dataHora >= consulta2.DataHoraConsulta && dataHora <= consulta2.DataHoraConsulta.AddMinutes(30) ||
                dataHora.AddMinutes(30) >= consulta2.DataHoraConsulta && dataHora.AddMinutes(30) <= consulta2.DataHoraConsulta.AddMinutes(30))
            {
                throw new ArgumentException("Já existe uma consulta agendada para esse horário!");
            }
        }

        var medico = await _db.Medicos.FindAsync(consulta.MedicoId);
        if (medico == null) throw new ArgumentException("Médico não encontrado!");

        var paciente = await _db.Pacientes.FindAsync(consulta.PacienteId);
        if (paciente == null) throw new ArgumentException("Paciente não encontrado!");

        _db.Consultas.Add(consulta);
        await _db.SaveChangesAsync();

        return _mapper.ToDTO(consulta);
    }

    public async Task<ConsultaReadDTO?> UpdateAsync(int id, ConsultaUpdateDTO dto)
    {
        var consulta = await _db.Consultas.FindAsync(id);
        if (consulta == null) return null;

        if (dto.PacienteId is not null)
        {
            var paciente = await _db.Pacientes.FindAsync((int)dto.PacienteId);
            if (paciente == null) throw new ArgumentException("Paciente não encontrado!");
            consulta.PacienteId = (int)dto.PacienteId;
        }

        if (dto.MedicoId is not null)
        {
            var medico = await _db.Medicos.FindAsync((int)dto.MedicoId);
            if (medico == null) throw new ArgumentException("Médico não encontrado!");
            consulta.MedicoId = (int)dto.MedicoId;
        }

        if (dto.DataHoraConsulta is not null)
        {
            if (dto.DataHoraConsulta < DateTime.Now) throw new ArgumentException("Data e hora da consulta não pode ser no passado!");
            consulta.DataHoraConsulta = (DateTime)dto.DataHoraConsulta;
        }

        var consultasLista = await _db.Consultas.ToListAsync();
        var consultasFiltradas = consultasLista
            .Where(c => c.DataHoraConsulta >= DateTime.Now)
            .Where(c => (c.MedicoId == consulta.MedicoId || c.PacienteId == consulta.PacienteId) && c.Id != consulta.Id)
            .ToList();

        DateTime dataHora = (DateTime)consulta.DataHoraConsulta;
        foreach (var consulta2 in consultasFiltradas)
        {
            if (dataHora >= consulta2.DataHoraConsulta && dataHora <= consulta2.DataHoraConsulta.AddMinutes(30) ||
                dataHora.AddMinutes(30) >= consulta2.DataHoraConsulta && dataHora.AddMinutes(30) <= consulta2.DataHoraConsulta.AddMinutes(30))
            {
                throw new ArgumentException("Já existe uma consulta agendada para esse horário!");
            }
        }

        await _db.SaveChangesAsync();
        return _mapper.ToDTO(consulta);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var consulta = await _db.Consultas.FindAsync(id);
        if (consulta == null) return false;
        _db.Consultas.Remove(consulta);
        await _db.SaveChangesAsync();
        return true;
    }
}
