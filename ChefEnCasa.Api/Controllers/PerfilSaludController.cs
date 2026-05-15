using AutoMapper;
using ChefEnCasa.Application.DTOs;
using ChefEnCasa.Domain.Entities;
using ChefEnCasa.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ChefEnCasa.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PerfilSaludController(IPerfilSaludService perfilService, IMapper mapper) : ControllerBase
    {
        private readonly IPerfilSaludService _perfilService = perfilService;
        private readonly IMapper _mapper = mapper;

        [HttpGet("{usuarioId}")]
        public async Task<IActionResult> ObtenerPerfil(Guid usuarioId)
        {
            try
            {
                var perfilDb = await _perfilService.ObtenerPerfilAsync(usuarioId);

                if (perfilDb == null)
                    return NotFound(new { message = "El usuario aún no ha configurado su perfil de salud." });

                var perfilDto = _mapper.Map<PerfilSaludDTO>(perfilDb);
                return Ok(perfilDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener el perfil", detalle = ex.Message });
            }
        }

        [HttpPost("actualizar")]
        public async Task<IActionResult> ActualizarPerfil([FromBody] ActualizarPerfilSaludDTO dto)
        {
            try
            {
                // Mapeamos lo básico (Peso, Altura, Dietas)
                var perfilEntidad = _mapper.Map<PerfilSalud>(dto);

                // Le pasamos la entidad y la lista de IDs de alergias al servicio
                var resultado = await _perfilService.ActualizarPerfilAsync(perfilEntidad, dto.AlergiasIngredienteIds);

                if (resultado)
                    return Ok(new { message = "Perfil de salud y alergias actualizados correctamente." });

                return BadRequest(new { message = "No se pudo actualizar el perfil." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", detalle = ex.Message });
            }
        }
    }
}