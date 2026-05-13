using ChefEnCasa.Application.DTOs;
using ChefEnCasa.Domain.Entities;
using ChefEnCasa.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ChefEnCasa.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlmacenController(IAlmacenService almacenService) : ControllerBase
    {
        private readonly IAlmacenService _almacenService = almacenService;

        [HttpPost("agregar")]
        public async Task<IActionResult> AgregarAlmacen([FromBody] AgregarAlmacenDTO dto)
        {
            try
            {
                // Mapeo manual (o puedes usar AutoMapper si lo agregas al AutoMapperProfile)
                var nuevoItemAlmacen = new Almacen
                {
                    UsuarioId = dto.UsuarioId,
                    IngredienteId = dto.IngredienteId,
                    EsPerecedero = dto.EsPerecedero,
                    FechaCaducidad = dto.FechaCaducidad
                };

                // Llamamos al servicio pasando la entidad y los strings de medida
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
    }
}