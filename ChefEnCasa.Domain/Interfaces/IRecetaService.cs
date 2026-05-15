using ChefEnCasa.Domain.Entities;

namespace ChefEnCasa.Domain.Interfaces
{
    public interface IRecetaService
    {
        Task<(List<Receta> Recetas, int TotalRegistros)> ObtenerRecetasPaginadasAsync(int pagina, int tamañoPagina, string? busqueda, Guid? usuarioId = null);

        Task<Receta?> ObtenerRecetaPorIdAsync(int recetaId);

        // Firma única y purificada: Devuelve ReporteCocina (Entidad de Dominio)
        Task<ReporteCocina> CocinarRecetaAsync(Guid usuarioId, int recetaId);

        // Agrega esta firma a tu interfaz
        Task<List<IngredienteFaltante>> ObtenerFaltantesParaRecetaAsync(Guid usuarioId, int recetaId);
    }
}