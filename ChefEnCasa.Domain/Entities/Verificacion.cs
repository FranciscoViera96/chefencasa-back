namespace ChefEnCasa.Domain.Entities
{
    public class Verificacion
    {
        public int VerificacionId { get; set; } // Esta puede quedar en INT si quieres, o pasarla a Guid. La dejé en INT como en tu foto.
        public Guid UsuarioId { get; set; }
        public string Token { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public DateTime FechaExpiracion { get; set; }

        // Opcional: Propiedad de navegación
        // public Usuario Usuario { get; set; }
    }
}