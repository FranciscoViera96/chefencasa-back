using AutoMapper;
using ChefEnCasa.Application.DTOs;
using ChefEnCasa.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ChefEnCasa.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecetaController(IRecetaService recetaService, IMapper mapper) : ControllerBase
    {
        private readonly IRecetaService _recetaService = recetaService;
        private readonly IMapper _mapper = mapper;

        [HttpGet]
        public async Task<IActionResult> GetRecetas([FromQuery] int pagina = 1, [FromQuery] int tamañoPagina = 20, [FromQuery] string? busqueda = null)
        {
            try
            {
                // 1. Obtenemos las entidades desde el Dominio
                var (recetasDb, total) = await _recetaService.ObtenerRecetasPaginadasAsync(pagina, tamañoPagina, busqueda);

                // 2. Mapeamos de Entidad a DTO aquí en el Controller
                var recetasDto = _mapper.Map<List<RecetaListDTO>>(recetasDb);

                // 3. Retornamos
                return Ok(new
                {
                    Data = recetasDto,
                    TotalRegistros = total,
                    PaginaActual = pagina,
                    TotalPaginas = (int)Math.Ceiling(total / (double)tamañoPagina)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener las recetas", detalle = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReceta(int id)
        {
            try
            {
                var recetaDb = await _recetaService.ObtenerRecetaPorIdAsync(id);

                if (recetaDb == null)
                    return NotFound(new { message = $"No se encontró la receta con ID {id}" });

                // Mapeamos al DTO detallado (que incluye los ingredientes)
                var recetaDto = _mapper.Map<RecetaDetalleDTO>(recetaDb);

                return Ok(recetaDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener el detalle de la receta", detalle = ex.Message });
            }
        }
    }
}