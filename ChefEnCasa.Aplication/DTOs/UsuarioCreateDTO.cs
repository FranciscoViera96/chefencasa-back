namespace ChefEnCasa.Application.DTOs
{
    public class UsuarioCreateDTO
    {
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? TelefonoPrefijo { get; set; }
        public string? TelefonoNumero { get; set; }
        public bool PoliciesAccepted { get; set; }
    }
}