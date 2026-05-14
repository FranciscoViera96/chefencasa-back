using ChefEnCasa.Domain.Entities;
using ChefEnCasa.Domain.Interfaces;
using Org.BouncyCastle.Crypto.Generators;

namespace ChefEnCasa.Application.Services
{
    // OJO: Aquí deberás inyectar tu IEmailService de ChefEnCasa cuando lo crees
    public class UsuarioService(IUsuarioRepository usuarioRepository , IEmailService emailService) : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository = usuarioRepository;
        private readonly IEmailService _emailService = emailService;

        public async Task<Usuario?> GetByEmailAsync(string email) => await _usuarioRepository.GetByEmailAsync(email);

        public async Task<bool> UpdateUserAsync(Usuario usuario) => await _usuarioRepository.UpdateUserAsync(usuario);

        public async Task<Verificacion?> ObtenerVerificacionAsync(Guid usuarioId, string token) => await _usuarioRepository.ObtenerVerificacionAsync(usuarioId, token);

        public async Task<bool> EliminarVerificacionAsync(Verificacion verificacion) => await _usuarioRepository.EliminarVerificacionAsync(verificacion);

        public async Task<Usuario?> LoginAsync(string email, string password)
        {
            var usuario = await _usuarioRepository.GetByEmailAsync(email)
                ?? throw new ArgumentException("Credenciales incorrectas.");

            // Usando BCrypt para comparar (O texto plano si aún no instalas BCrypt)
            if (!BCrypt.Net.BCrypt.Verify(password, usuario.PasswordHash))
            // if (usuario.PasswordHash != password) // (Versión texto plano temporal)
            {
                throw new ArgumentException("Credenciales incorrectas.");
            }

            if (!usuario.EmailVerificado)
                throw new UnauthorizedAccessException("Debe verificar su correo electrónico para acceder.");

            usuario.FechaUltimaSesion = DateTime.UtcNow;
            await _usuarioRepository.UpdateUserAsync(usuario);

            return usuario;
        }

        public async Task<bool> CreateUserAsync(Usuario usuario, string passwordClaro)
        {
            var existeEmail = await _usuarioRepository.ExisteEmailAsync(usuario.Email);
            if (existeEmail)
                throw new ArgumentException("Este correo ya se encuentra registrado.");

            // Guardado seguro con BCrypt
            usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(passwordClaro);
            // usuario.PasswordHash = passwordClaro; // (Versión texto plano temporal)

            usuario.FechaRegistro = DateTime.UtcNow;
            usuario.EmailVerificado = false;

            usuario.PerfilSalud = new PerfilSalud { UltimaActualizacion = DateTime.UtcNow };
            usuario.Suscripcion = new Suscripcion { EstadoPremium = false };

            var creado = await _usuarioRepository.CreateUserAsync(usuario);

            if (creado)
            {
                var codigoSecreto = new Random().Next(100000, 999999).ToString();

                await _usuarioRepository.CrearVerificacionAsync(new Verificacion
                {
                    UsuarioId = usuario.UsuarioId,
                    Token = codigoSecreto,
                    Tipo = "Email",
                    FechaExpiracion = DateTime.UtcNow.AddMinutes(15)
                });

                await _emailService.EnviarCorreoVerificacionAsync(usuario.Email, usuario.Nombre, codigoSecreto);
            }

            return creado;
        }

        public async Task<bool> ReenviarCodigoAsync(string email)
        {
            var usuario = await _usuarioRepository.GetByEmailAsync(email) ?? throw new ArgumentException("Usuario no encontrado.");
            if (usuario.EmailVerificado) throw new ArgumentException("La cuenta ya está verificada.");

            var codigoSecreto = new Random().Next(100000, 999999).ToString();

            await _usuarioRepository.CrearVerificacionAsync(new Verificacion
            {
                UsuarioId = usuario.UsuarioId,
                Token = codigoSecreto,
                Tipo = "Email",
                FechaExpiracion = DateTime.UtcNow.AddMinutes(15)
            });

            await _emailService.EnviarCorreoVerificacionAsync(usuario.Email, usuario.Nombre, codigoSecreto);
            return true;
        }

        public async Task<bool> SolicitarRecuperacionAsync(string email)
        {
            var usuario = await _usuarioRepository.GetByEmailAsync(email);
            if (usuario == null) return true; // Por seguridad no decimos si existe o no

            string codigoSecreto = new Random().Next(100000, 999999).ToString();
            usuario.ResetToken = codigoSecreto;
            usuario.ResetTokenExpiration = DateTime.UtcNow.AddMinutes(15);

            await _usuarioRepository.UpdateUserAsync(usuario);
            await _emailService.EnviarCorreoRecuperacionAsync(usuario.Email, usuario.Nombre, codigoSecreto);

            return true;
        }

        public async Task<bool> ResetearContrasenaAsync(string email, string token, string newPassword)
        {
            var usuario = await _usuarioRepository.GetByEmailAsync(email) ?? throw new ArgumentException("Código inválido o expirado.");

            if (usuario.ResetToken != token || usuario.ResetTokenExpiration == null || usuario.ResetTokenExpiration < DateTime.UtcNow)
                throw new ArgumentException("Código inválido o expirado.");

            usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            usuario.PasswordHash = newPassword; 

            usuario.ResetToken = null;
            usuario.ResetTokenExpiration = null;

            return await _usuarioRepository.UpdateUserAsync(usuario);
        }
    }
}