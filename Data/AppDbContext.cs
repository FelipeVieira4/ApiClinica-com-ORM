using Microsoft.EntityFrameworkCore;
using ApiClinica.Models;

namespace ApiClinica.Data;

public class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=clinica.db");
        }
    }

    public DbSet<Paciente> Pacientes { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Medico> Medicos { get; set; }
    public DbSet<Consulta> Consultas { get; set; }
}
