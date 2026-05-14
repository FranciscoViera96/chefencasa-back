using ChefEnCasa.Domain.Entities;

namespace ChefEnCasa.Domain.Interfaces
{
    public interface IUsuarioService
    {
        Task<Usuario?> GetByEmailAsync(string email);
        Task<Usuario?> LoginAsync(string email, string password);
        Task<bool> CreateUserAsync(Usuario usuario, string passwordClaro);

        // Nuevos métodos de seguridad
        Task<bool> ReenviarCodigoAsync(string email);
        Task<bool> SolicitarRecuperacionAsync(string email);
        Task<bool> ResetearContrasenaAsync(string email, string token, string newPassword);

        // Helper para el controller
        Task<bool> UpdateUserAsync(Usuario usuario);
        Task<Verificacion?> ObtenerVerificacionAsync(Guid usuarioId, string token);
        Task<bool> EliminarVerificacionAsync(Verificacion verificacion);
    }
}