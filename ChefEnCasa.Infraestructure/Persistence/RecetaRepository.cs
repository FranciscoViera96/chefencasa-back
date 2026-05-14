using ChefEnCasa.Domain.Entities;
using ChefEnCasa.Domain.Interfaces;
using ChefEnCasa.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace ChefEnCasa.Infrastructure.Persistence
{
    public class RecetaRepository(ChefEnCasaDbContext context) : IRecetaRepository
    {
        private readonly ChefEnCasaDbContext _context = context;

        public async Task<Receta?> ObtenerRecetaConIngredientesAsync(int recetaId)
        {
            return await _context.Recetas
                .Include(r => r.Ingredientes)
                    .ThenInclude(ri => ri.Ingrediente)
                .AsNoTracking() // Buena práctica para lecturas más rápidas
                .FirstOrDefaultAsync(r => r.RecetaId == recetaId);
        }

        public async Task<(List<Receta> Recetas, int Total)> ObtenerRecetasPaginadasAsync(int pagina, int tamañoPagina, string? busqueda)
        {
            var query = _context.Recetas.AsQueryable();

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                query = query.Where(r => r.Titulo.Contains(busqueda));
            }

            int total = await query.CountAsync();

            var recetas = await query
                .OrderBy(r => r.Titulo)
                .Skip((pagina - 1) * tamañoPagina)
                .Take(tamañoPagina)
                .AsNoTracking()
                .ToListAsync();

            return (recetas, total);
        }
    }
}