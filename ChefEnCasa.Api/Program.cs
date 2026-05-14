using ChefEnCasa.Aplication.Services;
using ChefEnCasa.Application.Interfaces;
using ChefEnCasa.Application.Mappings;
using ChefEnCasa.Application.Services;
using ChefEnCasa.Domain.Interfaces;
using ChefEnCasa.Infrastructure.Configurations;
using ChefEnCasa.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Base de Datos
builder.Services.AddDbContext<ChefEnCasaDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ChefEnCasaDbContext")));

// 2. AutoMapper
builder.Services.AddAutoMapper(cfg => { cfg.AddProfile<AutoMapperProfile>(); });

// 3. Inyección de Dependencias
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

builder.Services.AddScoped<IAlmacenRepository, AlmacenRepository>();
builder.Services.AddScoped<IAlmacenService, AlmacenService>();

builder.Services.AddScoped<IRecetaRepository, RecetaRepository>();
builder.Services.AddScoped<IRecetaService, RecetaService>();

// Añade esto donde están tus otros Scoped
builder.Services.AddHttpClient<ISpoonacularSyncService, SpoonacularSyncService>();
builder.Services.AddScoped<ISpoonacularSyncService, SpoonacularSyncService>();

// 4. Controladores y Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//EMAIL
builder.Services.AddScoped<IEmailService, EmailService>();

//PErfil salud
builder.Services.AddScoped<IPerfilSaludRepository, PerfilSaludRepository>();
builder.Services.AddScoped<IPerfilSaludService, PerfilSaludService>();


// 5. CORS Básico (Permite todo para evitar dolores de cabeza mientras desarrollamos la API)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configurar el pipeline HTTP
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

// Aplicar el CORS básico antes de los controladores
app.UseCors("AllowAll");

app.UseAuthorization();
app.MapControllers();

app.Run();