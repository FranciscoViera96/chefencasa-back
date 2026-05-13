using AutoMapper;
using ChefEnCasa.Application.DTOs;
using ChefEnCasa.Application.Interfaces;
using ChefEnCasa.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ChefEnCasa.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public UsuarioController(IUsuarioService usuarioService, IMapper mapper, IConfiguration config)
        {
            _usuarioService = usuarioService;
            _mapper = mapper;
            _config = config;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDTO>> Login([FromBody] LoginRequestDTO dto)
        {
            try
            {
                var usuario = await _usuarioService.LoginAsync(dto.Email, dto.Password);

                // Generación del Token JWT
                var tokenHandler = new JwtSecurityTokenHandler();
                var secretKey = _config["JwtSettings:Secret"]
                    ?? throw new InvalidOperationException("Falta configurar JwtSettings:Secret en el appsettings.json");

                var key = Encoding.ASCII.GetBytes(secretKey);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioId.ToString()),
                        new Claim(ClaimTypes.Email, usuario.Email)
                    }),
                    Expires = DateTime.UtcNow.AddDays(30), // En tu index.js de Node duraba 30 días
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var jwtString = tokenHandler.WriteToken(token);

                var response = new LoginResponseDTO
                {
                    Token = jwtString,
                    UsuarioId = usuario.UsuarioId,
                    Nombre = usuario.Nombre,
                    Email = usuario.Email,
                    Puntos = usuario.Puntos
                };

                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("create")]
        public async Task<ActionResult> Create([FromBody] UsuarioCreateDTO dto)
        {
            try
            {
                var usuario = _mapper.Map<Usuario>(dto);
                var resultado = await _usuarioService.CreateUserAsync(usuario, dto.Password);

                if (resultado) return Ok(new { message = "Cuenta creada exitosamente." });
                return BadRequest(new { message = "Error al crear la cuenta." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}