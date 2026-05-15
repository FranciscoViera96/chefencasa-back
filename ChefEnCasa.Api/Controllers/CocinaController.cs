using AutoMapper;
using ChefEnCasa.Aplication.DTOs;
using ChefEnCasa.Application.DTOs;
using ChefEnCasa.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ChefEnCasa.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CocinaController(IRecetaService recetaService, IMapper mapper) : ControllerBase
    {
        private readonly IRecetaService _recetaService = recetaService;
        private readonly IMapper _mapper = mapper; // Inyectamos AutoMapper

        [HttpPost("preparar")]
        public async Task<IActionResult> PrepararReceta([FromBody] CocinarDTO dto)
        {
            try
            {
                // 1. Obtenemos el Reporte de Dominio
                var reporteDomain = await _recetaService.CocinarRecetaAsync(dto.UsuarioId, dto.RecetaId);

                // 2. Mapeamos al DTO de la capa Application
                var resultadoDto = _mapper.Map<ResultadoCocinaDTO>(reporteDomain);

                // 3. Devolvemos el DTO
                return Ok(resultadoDto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Se quemó la comida en el servidor.", detalle = ex.Message });
            }
        }

        [HttpGet("evaluar-faltantes/{recetaId}")]
        public async Task<IActionResult> EvaluarFaltantes(int recetaId, [FromQuery] Guid usuarioId)
        {
            try
            {
                var faltantesDomain = await _recetaService.ObtenerFaltantesParaRecetaAsync(usuarioId, recetaId);

                // Mapeo al DTO (¡No olvides agregarlo al Profile!)
                var faltantesDto = _mapper.Map<List<IngredienteFaltanteDTO>>(faltantesDomain);

                return Ok(new
                {
                    PuedeCocinar = !faltantesDto.Any(),
                    Faltantes = faltantesDto
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}