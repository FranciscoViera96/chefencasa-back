namespace ChefEnCasa.Domain.Entities
{
    public class Suscripcion
    {
        public Guid SuscripcionId { get; set; } = Guid.NewGuid();
        public Guid UsuarioId { get; set; }
        public bool EstadoPremium { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }

        public Usuario Usuario { get; set; }
    }
}