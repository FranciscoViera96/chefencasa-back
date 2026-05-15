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
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.RecetaId == recetaId);
        }

        public async Task<(List<Receta> Recetas, int Total)> ObtenerRecetasPaginadasAsync(
            int pagina,
            int tamañoPagina,
            string? busqueda,
            Guid? usuarioId = null)
        {
            var query = _context.Recetas.AsQueryable();

            // 1. FILTRO DE SALUD AUTOMÁTICO
            if (usuarioId.HasValue)
            {
                var perfil = await _context.PerfilesSalud
                    .Include(p => p.Alergias)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.UsuarioId == usuarioId);

                if (perfil != null)
                {
                    // Filtro de Dieta
                    if (perfil.EsVegano) query = query.Where(r => r.EsVegano);
                    else if (perfil.EsVegetariano) query = query.Where(r => r.EsVegetariano);

                    if (perfil.EsCeliaco) query = query.Where(r => r.EsSinGluten);
                    if (perfil.IntoleranteLactosa) query = query.Where(r => r.EsSinLacteos);

                    // Filtro de Alergias
                    var idsAlergias = perfil.Alergias.Select(a => a.IngredienteId).ToList();
                    if (idsAlergias.Any())
                    {
                        // Excluimos las recetas que tengan ingredientes prohibidos
                        query = query.Where(r => !r.Ingredientes.Any(ri => idsAlergias.Contains(ri.IngredienteId)));
                    }
                }
            }

            // 2. Filtro de búsqueda por texto
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