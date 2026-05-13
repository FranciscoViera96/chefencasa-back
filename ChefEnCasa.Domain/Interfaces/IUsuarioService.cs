using ChefEnCasa.Domain.Entities;

namespace ChefEnCasa.Application.Interfaces
{
    public interface IUsuarioService
    {
        Task<Usuario?> GetByEmailAsync(string email);
        Task<Usuario?> LoginAsync(string email, string password);
        Task<bool> CreateUserAsync(Usuario usuario, string passwordClaro);
    }
}