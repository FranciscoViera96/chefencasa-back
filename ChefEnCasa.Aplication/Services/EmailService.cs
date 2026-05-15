using ChefEnCasa.Domain.Interfaces;
using MailKit.Security;
using MimeKit;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace ChefEnCasa.Aplication.Services // Ajusta el namespace si lo pones en Application
{
    public class EmailService : IEmailService
    {
        // TODO: Recuerda cambiar esto por un correo de Gmail de Chef en Casa cuando lo crees
        private readonly string _emailOrigen = "don.wea.501@gmail.com";
        private readonly string _passwordOrigen = "bujx ughk aokv nvhl";

        public async Task EnviarCorreoVerificacionAsync(string destinatario, string nombreUsuario, string codigo)
        {
            var mensaje = new MimeMessage();
            mensaje.From.Add(new MailboxAddress("Chef en Casa", _emailOrigen));
            mensaje.To.Add(new MailboxAddress(nombreUsuario, destinatario));
            mensaje.Subject = "👨‍🍳 Verifica tu cuenta en Chef en Casa";

            // Plantilla HTML con estilo Chef (Tonos naranjas y limpios)
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
                <div style='background-color: #f8f9fa; color: #212529; padding: 40px; font-family: Arial, sans-serif; border-radius: 10px; max-width: 500px; margin: 0 auto; border: 1px solid #dee2e6;'>
                    <h2 style='color: #fd7e14; text-align: center;'>¡Bienvenido a tu cocina virtual, {nombreUsuario}!</h2>
                    <p style='color: #495057; text-align: center; font-size: 16px;'>Tu cuenta ha sido creada. Utiliza el siguiente código para encender los fogones y activar tu acceso:</p>
                    
                    <div style='background-color: #ffffff; padding: 20px; border-radius: 8px; text-align: center; margin: 30px 0; border: 2px dashed #fd7e14;'>
                        <h1 style='color: #fd7e14; letter-spacing: 10px; margin: 0; font-size: 32px;'>{codigo}</h1>
                    </div>
                    
                    <p style='color: #6c757d; font-size: 12px; text-align: center;'>Este código caducará en 15 minutos.</p>
                    <p style='color: #6c757d; font-size: 12px; text-align: center;'><strong>Chef en Casa © 2026</strong></p>
                </div>"
            };

            mensaje.Body = bodyBuilder.ToMessageBody();

            using var clienteSmtp = new SmtpClient();
            try
            {
                //await clienteSmtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                await clienteSmtp.ConnectAsync("smtp.gmail.com", 465, SecureSocketOptions.SslOnConnect);
                await clienteSmtp.AuthenticateAsync(_emailOrigen, _passwordOrigen);
                await clienteSmtp.SendAsync(mensaje);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error enviando correo: {ex.Message}");
            }
            finally
            {
                await clienteSmtp.DisconnectAsync(true);
            }
        }

        public async Task EnviarCorreoRecuperacionAsync(string destinatario, string nombreUsuario, string codigo)
        {
            var mensaje = new MimeMessage();
            mensaje.From.Add(new MailboxAddress("Chef en Casa", _emailOrigen));
            mensaje.To.Add(new MailboxAddress(nombreUsuario, destinatario));
            mensaje.Subject = "🔐 Recuperación de Contraseña";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
                <div style='background-color: #f8f9fa; color: #212529; padding: 40px; font-family: Arial, sans-serif; border-radius: 10px; max-width: 500px; margin: 0 auto; border: 1px solid #dee2e6;'>
                    <h2 style='color: #198754; text-align: center;'>Recuperación de Acceso</h2>
                    <p style='color: #495057; text-align: center; font-size: 16px;'>Hola {nombreUsuario}, parece que olvidaste la receta para entrar. Usa este código temporal para crear una nueva contraseña:</p>
                    
                    <div style='background-color: #ffffff; padding: 20px; border-radius: 8px; text-align: center; margin: 30px 0; border: 2px dashed #198754;'>
                        <h1 style='color: #198754; letter-spacing: 10px; margin: 0; font-size: 32px;'>{codigo}</h1>
                    </div>
                    
                    <p style='color: #6c757d; font-size: 12px; text-align: center;'>Este código expirará en 15 minutos. Si no fuiste tú, ignora este correo.</p>
                </div>"
            };

            mensaje.Body = bodyBuilder.ToMessageBody();

            using var clienteSmtp = new SmtpClient();
            try
            {
                //await clienteSmtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                await clienteSmtp.ConnectAsync("smtp.gmail.com", 465, SecureSocketOptions.SslOnConnect);
                await clienteSmtp.AuthenticateAsync(_emailOrigen, _passwordOrigen);
                await clienteSmtp.SendAsync(mensaje);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error enviando correo de recuperación: {ex.Message}");
            }
            finally
            {
                await clienteSmtp.DisconnectAsync(true);
            }
        }
    }
}