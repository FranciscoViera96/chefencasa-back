using ChefEnCasa.Domain.Entities;
using ChefEnCasa.Domain.Interfaces;

namespace ChefEnCasa.Application.Services
{
    public class RecetaService(IRecetaRepository recetaRepository) : IRecetaService
    {
        private readonly IRecetaRepository _recetaRepository = recetaRepository;

        public async Task<(List<Receta> Recetas, int TotalRegistros)> ObtenerRecetasPaginadasAsync(int pagina, int tamañoPagina, string? busqueda)
        {
            return await _recetaRepository.ObtenerRecetasPaginadasAsync(pagina, tamañoPagina, busqueda);
        }

        public async Task<Receta?> ObtenerRecetaPorIdAsync(int recetaId)
        {
            return await _recetaRepository.ObtenerRecetaConIngredientesAsync(recetaId);
        }

        public async Task<bool> CocinarRecetaAsync(Guid usuarioId, int recetaId)
        {
            throw new NotImplementedException();
        }
    }
}