using ChefEnCasa.Domain.Entities;
using System.Threading.Tasks;

namespace ChefEnCasa.Domain.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> GetByIdAsync(Guid id); // En la interfaz
        Task<Usuario?> GetByEmailAsync(string email); // Agregamos el ?
        Task<bool> CreateUserAsync(Usuario usuario); // Estilo Hrno
        Task<bool> UpdateUserAsync(Usuario usuario); // Para el futuro
    }
}