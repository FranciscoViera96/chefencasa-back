using ChefEnCasa.Domain.Entities;

namespace ChefEnCasa.Domain.Interfaces
{
    public interface IRecetaRepository
    {
        Task<Receta?> ObtenerRecetaConIngredientesAsync(int recetaId);

        // Agregamos el usuarioId opcional aquí
        Task<(List<Receta> Recetas, int Total)> ObtenerRecetasPaginadasAsync(int pagina, int tamañoPagina, string? busqueda, Guid? usuarioId = null);
    }
}