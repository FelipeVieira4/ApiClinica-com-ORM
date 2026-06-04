using ApiClinica.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ApiClinica.Services;
using ApiClinica.Interfaces;
using ApiClinica.Mappers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=clinica.db")
);

// Mappers and services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPacienteMapper, PacienteMapperImpl>();
builder.Services.AddScoped<IPacienteService, PacienteService>();
builder.Services.AddScoped<IMedicoMapper, MedicoMapperImpl>();
builder.Services.AddScoped<IMedicoService, MedicoService>();
builder.Services.AddScoped<IConsultaMapper, ConsultaMapperImpl>();
builder.Services.AddScoped<IConsultaService, ConsultaService>();

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "change_this_secret";
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];
var key = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = new SymmetricSecurityKey(key),
			ValidateIssuer = !string.IsNullOrEmpty(jwtIssuer),
			ValidIssuer = jwtIssuer,
			ValidateAudience = !string.IsNullOrEmpty(jwtAudience),
			ValidAudience = jwtAudience,
			ValidateLifetime = true
		};
	});

builder.Services.AddAuthorization();

var app = builder.Build();

// Ensure DB exists and create required users (admin and regular user)
using (var scope = app.Services.CreateScope())
{
	var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
	db.Database.EnsureCreated();

	var auth = scope.ServiceProvider.GetRequiredService<IAuthService>();
	if (!db.Usuarios.Any(u => u.Username == "admin"))
	{
		// default admin credentials: admin / admin123
		auth.RegisterAsync(new ApiClinica.DTOs.RegisterDTO { Username = "admin", Password = "admin123", Role = "Admin" }).GetAwaiter().GetResult();
	}

	if (!db.Usuarios.Any(u => u.Username == "user"))
	{
		// default regular user: user / user123
		auth.RegisterAsync(new ApiClinica.DTOs.RegisterDTO { Username = "user", Password = "user123", Role = "User" }).GetAwaiter().GetResult();
	}
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();