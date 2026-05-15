using ChefEnCasa.Domain.Entities;
using ChefEnCasa.Domain.Interfaces;
using ChefEnCasa.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace ChefEnCasa.Infrastructure.Persistence
{
    public class PerfilSaludRepository(ChefEnCasaDbContext context) : IPerfilSaludRepository
    {
        private readonly ChefEnCasaDbContext _context = context;

        public async Task<PerfilSalud?> ObtenerPorUsuarioAsync(Guid usuarioId)
        {
            return await _context.PerfilesSalud
                .Include(p => p.Alergias)
                    .ThenInclude(a => a.Ingrediente) // Traemos el nombre del ingrediente
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.UsuarioId == usuarioId);
        }

        public async Task<bool> CrearOActualizarAsync(PerfilSalud perfil, List<int> nuevasAlergiasIds)
        {
            // 1. Buscamos si el usuario ya tiene un perfil
            var perfilDb = await _context.PerfilesSalud
                .Include(p => p.Alergias)
                .FirstOrDefaultAsync(p => p.UsuarioId == perfil.UsuarioId);

            // ==============================================================
            // 🧠 ALGORITMO DE EXPANSIÓN INTELIGENTE DE ALERGIAS
            // ==============================================================
            var idsExpandidos = new HashSet<int>(nuevasAlergiasIds);

            if (nuevasAlergiasIds.Any())
            {
                var nombresBase = await _context.Ingredientes
                    .Where(i => nuevasAlergiasIds.Contains(i.IngredienteId))
                    .Select(i => i.NombreEspanol.ToLower())
                    .ToListAsync();

                foreach (var nombre in nombresBase)
                {
                    // Tomamos la primera palabra
                    var palabraClave = nombre.Split(' ')[0];

                    // Singularizamos a la fuerza bruta
                    if (palabraClave.EndsWith("es"))
                        palabraClave = palabraClave.Substring(0, palabraClave.Length - 2);
                    else if (palabraClave.EndsWith("s"))
                        palabraClave = palabraClave.Substring(0, palabraClave.Length - 1);

                    // Seguro anti-palabras cortas (ej: "sal")
                    if (palabraClave.Length > 3)
                    {
                        var idsRelacionados = await _context.Ingredientes
                            .Where(i => i.NombreEspanol.ToLower().Contains(palabraClave))
                            .Select(i => i.IngredienteId)
                            .ToListAsync();

                        foreach (var id in idsRelacionados)
                        {
                            idsExpandidos.Add(id);
                        }
                    }
                }
            }
            // ==============================================================

            if (perfilDb == null)
            {
                // CREAR NUEVO
                perfil.Alergias = idsExpandidos.Select(id => new PerfilAlergia { IngredienteId = id }).ToList();
                await _context.PerfilesSalud.AddAsync(perfil);
            }
            else
            {
                // ACTUALIZAR EXISTENTE
                perfilDb.Peso = perfil.Peso;
                perfilDb.Altura = perfil.Altura;
                perfilDb.IMC = perfil.IMC;
                perfilDb.NecesidadCalorica = perfil.NecesidadCalorica;
                perfilDb.TMB = perfil.TMB;
                perfilDb.EsVegetariano = perfil.EsVegetariano;
                perfilDb.EsVegano = perfil.EsVegano;
                perfilDb.EsCeliaco = perfil.EsCeliaco;
                perfilDb.IntoleranteLactosa = perfil.IntoleranteLactosa;
                perfilDb.UltimaActualizacion = DateTime.UtcNow;

                // TRUCO DE TECH LEAD: Borramos las alergias viejas y metemos las nuevas EXPANDIDAS
                _context.PerfilAlergias.RemoveRange(perfilDb.Alergias);

                perfilDb.Alergias = idsExpandidos.Select(id => new PerfilAlergia
                {
                    PerfilSaludId = perfilDb.PerfilSaludId,
                    IngredienteId = id
                }).ToList();

                _context.PerfilesSalud.Update(perfilDb);
            }

            return await _context.SaveChangesAsync() > 0;
        }
    }
}