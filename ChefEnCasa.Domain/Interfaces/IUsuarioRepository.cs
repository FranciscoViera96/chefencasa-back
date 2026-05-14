using ChefEnCasa.Domain.Entities;

namespace ChefEnCasa.Domain.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> GetByIdAsync(Guid id);
        Task<Usuario?> GetByEmailAsync(string email);
        Task<bool> CreateUserAsync(Usuario usuario);
        Task<bool> UpdateUserAsync(Usuario usuario);
        Task<bool> ExisteEmailAsync(string email);

        // Verificaciones
        Task<bool> CrearVerificacionAsync(Verificacion verificacion);
        Task<Verificacion?> ObtenerVerificacionAsync(Guid usuarioId, string token);
        Task<bool> EliminarVerificacionAsync(Verificacion verificacion);
    }
}