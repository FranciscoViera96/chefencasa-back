using ChefEnCasa.Domain.Entities;

namespace ChefEnCasa.Domain.Interfaces
{
    public interface IRecetaService
    {
        Task<bool> CocinarRecetaAsync(Guid usuarioId, int recetaId);

        // Retorna la Entidad Receta, nada de DTOs
        Task<(List<Receta> Recetas, int TotalRegistros)> ObtenerRecetasPaginadasAsync(int pagina, int tamañoPagina, string? busqueda);

        Task<Receta?> ObtenerRecetaPorIdAsync(int recetaId);
    }
}