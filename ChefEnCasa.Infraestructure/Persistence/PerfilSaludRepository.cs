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

            if (perfilDb == null)
            {
                // CREAR NUEVO
                perfil.Alergias = nuevasAlergiasIds.Select(id => new PerfilAlergia { IngredienteId = id }).ToList();
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

                // TRUCO DE TECH LEAD: Borramos las alergias viejas y metemos las nuevas
                _context.PerfilAlergias.RemoveRange(perfilDb.Alergias);

                perfilDb.Alergias = nuevasAlergiasIds.Select(id => new PerfilAlergia
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