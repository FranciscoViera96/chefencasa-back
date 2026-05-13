namespace ChefEnCasa.Domain.Interfaces
{
    public interface IRecetaService
    {
        Task<bool> CocinarRecetaAsync(Guid usuarioId, int recetaId);
    }
}