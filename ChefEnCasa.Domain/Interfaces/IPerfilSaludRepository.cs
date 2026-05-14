using ChefEnCasa.Domain.Entities;

namespace ChefEnCasa.Domain.Interfaces
{
    public interface IPerfilSaludRepository
    {
        Task<PerfilSalud?> ObtenerPorUsuarioAsync(Guid usuarioId);
        Task<bool> CrearOActualizarAsync(PerfilSalud perfil, List<int> nuevasAlergiasIds);
    }
}