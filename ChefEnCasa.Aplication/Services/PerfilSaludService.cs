using ChefEnCasa.Domain.Entities;
using ChefEnCasa.Domain.Interfaces;

namespace ChefEnCasa.Application.Services
{
    public class PerfilSaludService(IPerfilSaludRepository perfilRepository) : IPerfilSaludService
    {
        private readonly IPerfilSaludRepository _perfilRepository = perfilRepository;

        public async Task<PerfilSalud?> ObtenerPerfilAsync(Guid usuarioId)
        {
            return await _perfilRepository.ObtenerPorUsuarioAsync(usuarioId);
        }

        public async Task<bool> ActualizarPerfilAsync(PerfilSalud perfil, List<int> alergiasIds)
        {
            return await _perfilRepository.CrearOActualizarAsync(perfil, alergiasIds);
        }
    }
}