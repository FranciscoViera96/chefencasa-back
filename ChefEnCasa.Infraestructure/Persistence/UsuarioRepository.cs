using ChefEnCasa.Domain.Entities;
using ChefEnCasa.Domain.Interfaces;
using ChefEnCasa.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ChefEnCasa.Infraestructure.Persistence
{
    public class UsuarioRepository(ChefEnCasaDbContext context) : IUsuarioRepository // Usando Primary Constructor como en Hrno
    {
        private readonly ChefEnCasaDbContext _context = context;

        // En el repositorio:
        public async Task<Usuario?> GetByIdAsync(Guid id)
        {
            return await _context.Usuarios
                .Include(u => u.PerfilSalud)
                .Include(u => u.Suscripcion)
                .FirstOrDefaultAsync(u => u.UsuarioId == id);
        }

        public async Task<Usuario?> GetByEmailAsync(string email)
        {
            return await _context.Usuarios
                .AsNoTracking() // Optimización estilo Hrno
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> CreateUserAsync(Usuario usuario)
        {
            await _context.Usuarios.AddAsync(usuario);
            return await _context.SaveChangesAsync() > 0; // Estilo Hrno
        }

        public async Task<bool> UpdateUserAsync(Usuario usuario)
        {
            _context.Usuarios.Update(usuario);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}