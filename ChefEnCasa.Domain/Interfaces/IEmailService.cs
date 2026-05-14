namespace ChefEnCasa.Domain.Interfaces
{
    public interface IEmailService
    {
        Task EnviarCorreoVerificacionAsync(string destinatario, string nombreUsuario, string codigo);
        Task EnviarCorreoRecuperacionAsync(string destinatario, string nombreUsuario, string codigo);
    }
}