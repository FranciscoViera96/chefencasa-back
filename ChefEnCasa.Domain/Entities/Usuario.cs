namespace ChefEnCasa.Domain.Entities
{
    public class Usuario
    {
        public Guid UsuarioId { get; set; } = Guid.NewGuid(); // <-- Cambiado a Guid
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty; // Lo mantenemos string por el BCrypt
        public string? TelefonoPrefijo { get; set; }
        public string? TelefonoNumero { get; set; }
        public int Puntos { get; set; }
        public bool PoliciesAccepted { get; set; }

        // Campos de Verificación (Traídos de Hrno)
        public bool EmailVerificado { get; set; }
        public bool TelefonoVerificado { get; set; }
        public string? ResetToken { get; set; }
        public DateTime? ResetTokenExpiration { get; set; }

        public DateTime FechaRegistro { get; set; }
        public DateTime? FechaUltimaSesion { get; set; }

        // Propiedades de Navegación
        public PerfilSalud PerfilSalud { get; set; }
        public Suscripcion Suscripcion { get; set; }
    }
}