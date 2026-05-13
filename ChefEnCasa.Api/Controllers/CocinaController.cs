using ChefEnCasa.Application.DTOs;
using ChefEnCasa.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ChefEnCasa.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CocinaController(IRecetaService recetaService) : ControllerBase
    {
        private readonly IRecetaService _recetaService = recetaService;

        [HttpPost("preparar")]
        public async Task<IActionResult> PrepararReceta([FromBody] CocinarDTO dto)
        {
            try
            {
                var resultado = await _recetaService.CocinarRecetaAsync(dto.UsuarioId, dto.RecetaId);
                return Ok(new { message = "¡Buen provecho! Receta preparada exitosamente." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al cocinar.", detalle = ex.Message });
            }
        }
    }
}