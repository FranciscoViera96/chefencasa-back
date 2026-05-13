using ChefEnCasa.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
    }
}