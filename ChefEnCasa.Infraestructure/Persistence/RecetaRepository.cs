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
                    .ThenInclude(ri => ri.Ingrediente) // IMPORTANTE: Traer también el nombre del ingrediente para los mensajes de error
                .FirstOrDefaultAsync(r => r.RecetaId == recetaId);
        }
    }
}