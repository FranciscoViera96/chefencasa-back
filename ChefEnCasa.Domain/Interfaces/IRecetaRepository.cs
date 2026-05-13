using ChefEnCasa.Domain.Entities;

namespace ChefEnCasa.Domain.Interfaces
{
    public interface IRecetaRepository
    {
        Task<Receta?> ObtenerRecetaConIngredientesAsync(int recetaId);
        // Aquí luego agregaremos métodos como: ListarRecetasAsync(), CrearRecetaAsync()
    }
}