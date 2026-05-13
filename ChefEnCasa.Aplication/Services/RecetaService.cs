using ChefEnCasa.Domain.Entities;
using ChefEnCasa.Domain.Interfaces;
using ChefEnCasa.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace ChefEnCasa.Application.Services
{
    public class RecetaService(
        IRecetaRepository recetaRepository,
        IAlmacenRepository almacenRepository,
        ChefEnCasaDbContext context) : IRecetaService
    {
        private readonly IRecetaRepository _recetaRepository = recetaRepository;
        private readonly IAlmacenRepository _almacenRepository = almacenRepository;
        private readonly ChefEnCasaDbContext _context = context;

        public async Task<bool> CocinarRecetaAsync(Guid usuarioId, int recetaId)
        {
            var receta = await _recetaRepository.ObtenerRecetaConIngredientesAsync(recetaId)
                ?? throw new ArgumentException("La receta no existe.");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                foreach (var req in receta.Ingredientes)
                {
                    decimal faltante = req.CantidadEnGramosOMl;

                    // Buscamos lotes FEFO
                    var lotes = await _context.Almacenes
                        .Where(a => a.UsuarioId == usuarioId && a.IngredienteId == req.IngredienteId && a.CantidadEnGramosOMl > 0)
                        .OrderBy(a => a.EsPerecedero ? 0 : 1)
                        .ThenBy(a => a.FechaCaducidad ?? DateTime.MaxValue)
                        .ToListAsync();

                    if (lotes.Sum(l => l.CantidadEnGramosOMl) < faltante)
                    {
                        throw new InvalidOperationException($"Stock insuficiente de {req.Ingrediente.NombreEspanol}.");
                    }

                    foreach (var lote in lotes)
                    {
                        if (faltante <= 0) break;

                        if (lote.CantidadEnGramosOMl >= faltante)
                        {
                            lote.CantidadEnGramosOMl -= faltante;
                            faltante = 0;
                            _context.Almacenes.Update(lote);
                        }
                        else
                        {
                            faltante -= lote.CantidadEnGramosOMl;
                            _context.Almacenes.Remove(lote);
                        }
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}