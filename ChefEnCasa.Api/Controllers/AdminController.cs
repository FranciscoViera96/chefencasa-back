using ChefEnCasa.Application.Interfaces;
using ChefEnCasa.Domain.Constants; 
using ChefEnCasa.Infrastructure.Configurations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChefEnCasa.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController(ISpoonacularSyncService syncService) : ControllerBase
    {
        private readonly ISpoonacularSyncService _syncService = syncService;

        [HttpPost("robar-recetas")]
        public async Task<IActionResult> SincronizarSpoonacular([FromQuery] string letra = "a", [FromQuery] int cantidad = 10, [FromQuery] int offset = 0)
        {
            try
            {
                int creadas = await _syncService.SincronizarRecetasAsync(letra, cantidad, offset);
                return Ok(new { message = $"Sincronización exitosa. Se guardaron {creadas} recetas nuevas en la base de datos local." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error en la sincronización", error = ex.Message });
            }
        }

        [HttpPost("reajustar-medidas")]
        public async Task<IActionResult> ReajustarMedidas([FromServices] ChefEnCasaDbContext context)
        {
            try
            {
                // Lo hacemos asíncrono (ToListAsync) para no colgar la API cuando tengamos 5000 ingredientes
                var todosLosIngredientes = await context.RecetaIngredientes.ToListAsync();

                foreach (var item in todosLosIngredientes)
                {
                    // Pasamos la cantidad y unidad original por la calculadora
                    item.CantidadEnGramosOMl = ConversionesMedidas.ConvertirABase(item.Cantidad, item.UnidadMedida);
                }

                context.RecetaIngredientes.UpdateRange(todosLosIngredientes);
                await context.SaveChangesAsync();

                return Ok(new { message = $"Mantenimiento exitoso. Se reajustaron y sanitizaron las medidas de {todosLosIngredientes.Count} ingredientes en la BD." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al reajustar medidas", detalle = ex.Message });
            }
        }
    }
}