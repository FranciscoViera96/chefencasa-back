using ChefEnCasa.Application.Interfaces;
using ChefEnCasa.Domain.Entities;
using ChefEnCasa.Domain.Interfaces;

namespace ChefEnCasa.Application.Services
{
    public class UsuarioService(IUsuarioRepository usuarioRepository) : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository = usuarioRepository;

        public async Task<Usuario?> GetByEmailAsync(string email)
        {
            return await _usuarioRepository.GetByEmailAsync(email);
        }

        public async Task<Usuario?> LoginAsync(string email, string password)
        {
            var usuario = await _usuarioRepository.GetByEmailAsync(email)
                ?? throw new ArgumentException("Credenciales incorrectas.");

            // Comparación de contraseña en TEXTO PLANO (Temporal)
            if (usuario.PasswordHash != password)
            {
                throw new ArgumentException("Credenciales incorrectas.");
            }

            usuario.FechaUltimaSesion = DateTime.UtcNow;
            await _usuarioRepository.UpdateUserAsync(usuario);

            return usuario;
        }

        public async Task<bool> CreateUserAsync(Usuario usuario, string passwordClaro)
        {
            var existeEmail = await _usuarioRepository.GetByEmailAsync(usuario.Email);
            if (existeEmail != null)
                throw new ArgumentException("Este correo ya se encuentra registrado.");

            // Guardado en TEXTO PLANO (Temporal)
            usuario.PasswordHash = passwordClaro;
            usuario.FechaRegistro = DateTime.UtcNow;

            // Inicializar entidades hijas (Relación 1 a 1)
            usuario.PerfilSalud = new PerfilSalud { UltimaActualizacion = DateTime.UtcNow };
            usuario.Suscripcion = new Suscripcion { EstadoPremium = false };

            return await _usuarioRepository.CreateUserAsync(usuario);
        }
    }
}