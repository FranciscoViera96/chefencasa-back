namespace ChefEnCasa.Application.Interfaces
{
    public interface ISpoonacularSyncService
    {
        Task<int> SincronizarRecetasAsync(string letraBusqueda, int cantidadMaxima, int offset);
    }
}