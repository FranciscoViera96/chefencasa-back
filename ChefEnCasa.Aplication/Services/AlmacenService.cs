using ChefEnCasa.Application.DTOs;
using ChefEnCasa.Domain.Constants;
using ChefEnCasa.Domain.Entities;
using ChefEnCasa.Domain.Interfaces;
using ChefEnCasa.Infrastructure.Configurations; // Para usar el DbContext rápido y sacar la vida útil
using Microsoft.EntityFrameworkCore;

namespace ChefEnCasa.Application.Services
{
    public class AlmacenService(IAlmacenRepository almacenRepository, ChefEnCasaDbContext context) : IAlmacenService
    {
        private readonly IAlmacenRepository _almacenRepository = almacenRepository;
        private readonly ChefEnCasaDbContext _context = context;

        public async Task<bool> AgregarOActualizarIngredienteAsync(Almacen almacenItem, decimal cantidadOriginal, string unidadOriginal)
        {
            decimal cantidadNormalizada = ConversionesMedidas.ConvertirABase(cantidadOriginal, unidadOriginal);

            // 1. Si no viene fecha, calculamos la estándar del catálogo
            if (!almacenItem.FechaCaducidad.HasValue)
            {
                var ingredienteCatalogo = await _context.Ingredientes.FindAsync(almacenItem.IngredienteId);
                if (ingredienteCatalogo?.DiasVidaUtilEstimada != null)
                {
                    almacenItem.FechaCaducidad = DateTime.UtcNow.Date.AddDays(ingredienteCatalogo.DiasVidaUtilEstimada.Value);
                }
            }

            // 2. Buscamos si ya existe un lote con la MISMA fecha de caducidad
            var loteExistente = await _almacenRepository.ObtenerLoteEspecificoAsync(
                almacenItem.UsuarioId,
                almacenItem.IngredienteId,
                almacenItem.FechaCaducidad);

            if (loteExistente != null)
            {
                loteExistente.CantidadEnGramosOMl += cantidadNormalizada;
                loteExistente.FechaIngreso = DateTime.UtcNow;
                return await _almacenRepository.ActualizarAsync(loteExistente);
            }

            // 3. Si la fecha es distinta (o es el primero), creamos fila nueva
            almacenItem.CantidadEnGramosOMl = cantidadNormalizada;
            almacenItem.FechaIngreso = DateTime.UtcNow;
            return await _almacenRepository.AgregarAsync(almacenItem);
        }
        //public async Task<bool> AgregarOActualizarListaAsync(List<AgregarAlmacenDTO> listaIngredientes)
        //{
        //    bool exitoTotal = true;

        //    foreach (var dto in listaIngredientes)
        //    {
        //        var nuevoItemAlmacen = new Almacen
        //        {
        //            UsuarioId = dto.UsuarioId,
        //            IngredienteId = dto.IngredienteId,
        //            EsPerecedero = dto.EsPerecedero,
        //            FechaCaducidad = dto.FechaCaducidad
        //        };

        //        // Reutilizamos tu método que ya sabe calcular fechas y sumar gramos
        //        var resultado = await AgregarOActualizarIngredienteAsync(nuevoItemAlmacen, dto.Cantidad, dto.UnidadDeMedida);

        //        if (!resultado)
        //            exitoTotal = false; // Si falla uno, lo marcamos (aunque podríamos manejarlo mejor, para el MVP sirve)
        //    }

        //    return exitoTotal;
        //}
    }
}