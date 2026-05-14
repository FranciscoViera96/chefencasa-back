using AutoMapper;
using ChefEnCasa.Application.DTOs;
using ChefEnCasa.Domain.Entities;
using ChefEnCasa.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ChefEnCasa.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController(IUsuarioService usuarioService, IMapper mapper, IConfiguration config) : ControllerBase
    {
        private readonly IUsuarioService _usuarioService = usuarioService;
        private readonly IMapper _mapper = mapper;
        private readonly IConfiguration _config = config;

        // DTOs rápidos para endpoints sencillos
        public class ReenviarDto { public string Email { get; set; } = string.Empty; }
        public class VerificarDto { public string Email { get; set; } = string.Empty; public string Token { get; set; } = string.Empty; }
        public class ForgotPasswordDto { public string Email { get; set; } = string.Empty; }
        public class ResetPasswordDto { public string Email { get; set; } = string.Empty; public string Token { get; set; } = string.Empty; public string NewPassword { get; set; } = string.Empty; }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDTO>> Login([FromBody] LoginRequestDTO dto)
        {
            try
            {
                var usuario = await _usuarioService.LoginAsync(dto.Email, dto.Password);

                var secretKey = _config["JwtSettings:Secret"] ?? throw new InvalidOperationException("Falta JwtSettings:Secret");
                var key = Encoding.ASCII.GetBytes(secretKey);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioId.ToString()),
                        new Claim(ClaimTypes.Email, usuario.Email)
                    }),
                    Expires = DateTime.UtcNow.AddDays(30),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtString = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));

                return Ok(new LoginResponseDTO
                {
                    Token = jwtString,
                    UsuarioId = usuario.UsuarioId,
                    Nombre = usuario.Nombre,
                    Email = usuario.Email,
                    Puntos = usuario.Puntos
                });
            }
            catch (UnauthorizedAccessException ex) { return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message }); }
            catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpPost("create")]
        public async Task<ActionResult> Create([FromBody] UsuarioCreateDTO dto)
        {
            try
            {
                var usuario = _mapper.Map<Usuario>(dto);
                var resultado = await _usuarioService.CreateUserAsync(usuario, dto.Password);

                if (resultado) return Ok(new { message = "Cuenta creada exitosamente. Revisa tu correo." });
                return BadRequest(new { message = "Error al crear la cuenta." });
            }
            catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpPost("verificar")]
        public async Task<ActionResult> VerificarCuenta([FromBody] VerificarDto dto)
        {
            var usuario = await _usuarioService.GetByEmailAsync(dto.Email);
            if (usuario == null) return BadRequest(new { message = "Usuario no encontrado." });

            var verificacion = await _usuarioService.ObtenerVerificacionAsync(usuario.UsuarioId, dto.Token);
            if (verificacion == null) return BadRequest(new { message = "El código es incorrecto." });
            if (verificacion.FechaExpiracion < DateTime.UtcNow) return BadRequest(new { message = "El código ha expirado." });

            usuario.EmailVerificado = true;
            await _usuarioService.UpdateUserAsync(usuario);
            await _usuarioService.EliminarVerificacionAsync(verificacion);

            return Ok(new { message = "Cuenta activada exitosamente." });
        }

        [HttpPost("reenviar-codigo")]
        public async Task<IActionResult> ReenviarCodigo([FromBody] ReenviarDto request)
        {
            try
            {
                await _usuarioService.ReenviarCodigoAsync(request.Email);
                return Ok(new { message = "Código reenviado con éxito." });
            }
            catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            await _usuarioService.SolicitarRecuperacionAsync(dto.Email);
            return Ok(new { message = "Si el correo está registrado, recibirás un código en breve." });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            try
            {
                await _usuarioService.ResetearContrasenaAsync(dto.Email, dto.Token, dto.NewPassword);
                return Ok(new { message = "Contraseña actualizada correctamente." });
            }
            catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
        }
    }
}