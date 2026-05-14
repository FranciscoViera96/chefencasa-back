using AutoMapper;
using ChefEnCasa.Application.DTOs;
using ChefEnCasa.Domain.Entities;
using ChefEnCasa.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ChefEnCasa.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlmacenController(IAlmacenService almacenService, IAlmacenRepository almacenRepository, IMapper mapper) : ControllerBase
    {
        private readonly IAlmacenService _almacenService = almacenService;
        private readonly IAlmacenRepository _almacenRepository = almacenRepository;
        private readonly IMapper _mapper = mapper;

        [HttpGet("{usuarioId}")]
        public async Task<IActionResult> ObtenerAlmacen(Guid usuarioId)
        {
            try
            {
                // 1. Buscamos las entidades crudas (con la info del Ingrediente incluida)
                var almacenDb = await _almacenRepository.ObtenerAlmacenPorUsuarioAsync(usuarioId);

                // 2. Mapeamos a DTOs bonitos para Angular
                var almacenDto = _mapper.Map<List<AlmacenItemDTO>>(almacenDb);

                return Ok(almacenDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener la despensa.", detalle = ex.Message });
            }
        }

        [HttpPost("agregar")]
        public async Task<IActionResult> AgregarAlmacen([FromBody] AgregarAlmacenDTO dto)
        {
            try
            {
                // Mapeo manual hacia la Entidad de Dominio
                var nuevoItemAlmacen = new Almacen
                {
                    UsuarioId = dto.UsuarioId,
                    IngredienteId = dto.IngredienteId,
                    EsPerecedero = dto.EsPerecedero,
                    FechaCaducidad = dto.FechaCaducidad
                };

                // El servicio hace la magia de normalizar y calcular fechas
                var resultado = await _almacenService.AgregarOActualizarIngredienteAsync(nuevoItemAlmacen, dto.Cantidad, dto.UnidadDeMedida);

                if (resultado)
                    return Ok(new { message = "Ingrediente agregado a la despensa exitosamente." });

                return BadRequest(new { message = "No se pudo actualizar el almacén." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor.", detalle = ex.Message });
            }
        }

        [HttpPost("agregar-lista")]
        public async Task<IActionResult> AgregarListaAlmacen([FromBody] List<AgregarAlmacenDTO> listaDto)
        {
            try
            {
                if (listaDto == null || !listaDto.Any())
                    return BadRequest(new { message = "La lista de ingredientes está vacía." });

                bool huboErrores = false;

                foreach (var dto in listaDto)
                {
                    // Transformamos el DTO a Entidad aquí en el Controller
                    var itemAlmacen = new Almacen
                    {
                        UsuarioId = dto.UsuarioId,
                        IngredienteId = dto.IngredienteId,
                        EsPerecedero = dto.EsPerecedero,
                        FechaCaducidad = dto.FechaCaducidad
                    };

                    // Llamamos al servicio (que es puro y solo recibe Entidad + Primitivos)
                    var resultado = await _almacenService.AgregarOActualizarIngredienteAsync(
                        itemAlmacen,
                        dto.Cantidad,
                        dto.UnidadDeMedida
                    );

                    if (!resultado) huboErrores = true;
                }

                if (huboErrores)
                    return BadRequest(new { message = "Algunos ingredientes no pudieron ser procesados." });

                return Ok(new { message = "Todos los ingredientes fueron agregados a la despensa." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno", detalle = ex.Message });
            }
        }
    }
}