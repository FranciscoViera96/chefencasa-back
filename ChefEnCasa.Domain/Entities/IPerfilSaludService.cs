using ChefEnCasa.Domain.Entities;

namespace ChefEnCasa.Domain.Interfaces
{
    public interface IPerfilSaludService
    {
        Task<PerfilSalud?> ObtenerPerfilAsync(Guid usuarioId);
        Task<bool> ActualizarPerfilAsync(PerfilSalud perfil, List<int> alergiasIds);
    }
}