using ChefEnCasa.Domain.Entities;
using ChefEnCasa.Domain.Interfaces;
using ChefEnCasa.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace ChefEnCasa.Infrastructure.Persistence
{
    public class AlmacenRepository(ChefEnCasaDbContext context) : IAlmacenRepository
    {
        private readonly ChefEnCasaDbContext _context = context;

        public async Task<List<Almacen>> ObtenerAlmacenPorUsuarioAsync(Guid usuarioId)
        {
            return await _context.Almacenes
                .Include(a => a.Ingrediente) // Trae los datos del catálogo
                .Where(a => a.UsuarioId == usuarioId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Almacen?> ObtenerItemEspecificoAsync(Guid usuarioId, int ingredienteId)
        {
            return await _context.Almacenes
                .FirstOrDefaultAsync(a => a.UsuarioId == usuarioId && a.IngredienteId == ingredienteId);
        }

        public async Task<bool> AgregarAsync(Almacen almacen)
        {
            await _context.Almacenes.AddAsync(almacen);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ActualizarAsync(Almacen almacen)
        {
            _context.Almacenes.Update(almacen);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Almacen?> ObtenerLoteEspecificoAsync(Guid usuarioId, int ingredienteId, DateTime? fechaCaducidad)
        {
            // Buscamos coincidencia exacta de ingrediente y fecha de caducidad
            return await _context.Almacenes
                .FirstOrDefaultAsync(a => a.UsuarioId == usuarioId
                                       && a.IngredienteId == ingredienteId
                                       && a.FechaCaducidad == fechaCaducidad);
        }

        public async Task<List<Almacen>> ObtenerLotesParaIngredientesAsync(Guid usuarioId, List<int> ingredienteIds)
        {
            // Traemos todos los lotes del usuario que coincidan con los ingredientes de la receta
            return await _context.Almacenes
                .Where(a => a.UsuarioId == usuarioId && ingredienteIds.Contains(a.IngredienteId))
                .ToListAsync();
        }

        public async Task<bool> AplicarDescuentoFEFOAsync(List<Almacen> lotesAActualizar, List<Almacen> lotesAEliminar)
        {
            if (lotesAEliminar.Any())
                _context.Almacenes.RemoveRange(lotesAEliminar);

            if (lotesAActualizar.Any())
                _context.Almacenes.UpdateRange(lotesAActualizar);

            return await _context.SaveChangesAsync() > 0;
        }
    }
}