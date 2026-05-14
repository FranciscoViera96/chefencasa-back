using ChefEnCasa.Domain.Entities;

namespace ChefEnCasa.Domain.Interfaces
{
    public interface IRecetaRepository
    {
        Task<Receta?> ObtenerRecetaConIngredientesAsync(int recetaId);

        // Retorna la Entidad Receta, nada de DTOs
        Task<(List<Receta> Recetas, int Total)> ObtenerRecetasPaginadasAsync(int pagina, int tamañoPagina, string? busqueda);
    }
}