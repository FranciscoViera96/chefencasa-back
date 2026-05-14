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
            // 1. Normalizamos a gramos o ml
            decimal cantidadNormalizada = ConversionesMedidas.ConvertirABase(cantidadOriginal, unidadOriginal);

            // 2. Buscamos si el usuario ya tiene este ingrediente en su almacén
            var loteExistente = await _almacenRepository.ObtenerItemEspecificoAsync(almacenItem.UsuarioId, almacenItem.IngredienteId);

            if (loteExistente != null)
            {
                // REGLA KISS: Ya existe. Solo sumamos la cantidad.
                loteExistente.CantidadEnGramosOMl += cantidadNormalizada;
                loteExistente.FechaIngreso = DateTime.UtcNow;

                // Si el usuario nos mandó una nueva fecha (compró uno más fresco), la actualizamos
                if (almacenItem.FechaCaducidad.HasValue)
                {
                    loteExistente.FechaCaducidad = almacenItem.FechaCaducidad;
                }

                return await _almacenRepository.ActualizarAsync(loteExistente);
            }
            else
            {
                // ES NUEVO. Aplicamos la magia de la Fecha Automática.
                if (!almacenItem.FechaCaducidad.HasValue)
                {
                    // Buscamos el ingrediente en el catálogo para ver su vida útil
                    var ingredienteCatalogo = await _context.Ingredientes.FindAsync(almacenItem.IngredienteId);

                    if (ingredienteCatalogo != null && ingredienteCatalogo.DiasVidaUtilEstimada.HasValue)
                    {
                        almacenItem.FechaCaducidad = DateTime.UtcNow.AddDays(ingredienteCatalogo.DiasVidaUtilEstimada.Value);
                    }
                }

                almacenItem.CantidadEnGramosOMl = cantidadNormalizada;
                almacenItem.FechaIngreso = DateTime.UtcNow;

                return await _almacenRepository.AgregarAsync(almacenItem);
            }
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