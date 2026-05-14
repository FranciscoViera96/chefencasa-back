using ChefEnCasa.Domain.Entities;
using ChefEnCasa.Domain.Interfaces;
using ChefEnCasa.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace ChefEnCasa.Infrastructure.Persistence
{
    public class UsuarioRepository(ChefEnCasaDbContext context) : IUsuarioRepository
    {
        private readonly ChefEnCasaDbContext _context = context;

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
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> ExisteEmailAsync(string email)
        {
            return await _context.Usuarios.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> CreateUserAsync(Usuario usuario)
        {
            await _context.Usuarios.AddAsync(usuario);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateUserAsync(Usuario usuario)
        {
            _context.Usuarios.Update(usuario);
            return await _context.SaveChangesAsync() > 0;
        }

        // --- VERIFICACIONES ---
        public async Task<bool> CrearVerificacionAsync(Verificacion verificacion)
        {
            await _context.Verificaciones.AddAsync(verificacion);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Verificacion?> ObtenerVerificacionAsync(Guid usuarioId, string token)
        {
            return await _context.Verificaciones
                .FirstOrDefaultAsync(v => v.UsuarioId == usuarioId && v.Token == token);
        }

        public async Task<bool> EliminarVerificacionAsync(Verificacion verificacion)
        {
            _context.Verificaciones.Remove(verificacion);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}