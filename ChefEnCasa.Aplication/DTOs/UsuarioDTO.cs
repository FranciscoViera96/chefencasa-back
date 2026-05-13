namespace ChefEnCasa.Application.DTOs
{
    public class UsuarioDTO
    {
        public Guid UsuarioId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int Puntos { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}