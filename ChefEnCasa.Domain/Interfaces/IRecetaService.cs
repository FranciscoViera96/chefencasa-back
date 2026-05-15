using ChefEnCasa.Domain.Entities;

namespace ChefEnCasa.Domain.Interfaces
{
    public interface IRecetaService
    {
        Task<bool> CocinarRecetaAsync(Guid usuarioId, int recetaId);

        // Agregamos el usuarioId opcional
        Task<(List<Receta> Recetas, int TotalRegistros)> ObtenerRecetasPaginadasAsync(int pagina, int tamañoPagina, string? busqueda, Guid? usuarioId = null);

        Task<Receta?> ObtenerRecetaPorIdAsync(int recetaId);
    }
}