using ChefEnCasa.Domain.Constants;
using ChefEnCasa.Domain.Entities;
using ChefEnCasa.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace ChefEnCasa.Application.Services
{
    public class AlmacenService(IAlmacenRepository almacenRepository) : IAlmacenService
    {
        private readonly IAlmacenRepository _almacenRepository = almacenRepository;

        public async Task<bool> AgregarOActualizarIngredienteAsync(Almacen almacenItem, decimal cantidadOriginal, string unidadOriginal)
        {
            // 1. Normalizamos la cantidad usando nuestra clase mágica del Domain
            decimal cantidadNormalizada = ConversionesMedidas.ConvertirABase(cantidadOriginal, unidadOriginal);

            // 2. Buscamos si existe un lote exacto (Usuario + Ingrediente + Misma Fecha de Vencimiento)
            var loteExistente = await _almacenRepository.ObtenerItemEspecificoAsync(almacenItem.UsuarioId, almacenItem.IngredienteId);

            // Ajuste FEFO: Validar también la fecha de caducidad para agrupar lotes
            if (loteExistente != null && loteExistente.FechaCaducidad == almacenItem.FechaCaducidad)
            {
                // Si es el mismo lote, sumamos la cantidad
                loteExistente.CantidadEnGramosOMl += cantidadNormalizada;
                loteExistente.FechaIngreso = DateTime.UtcNow;

                return await _almacenRepository.ActualizarAsync(loteExistente);
            }
            else
            {
                // Si no existe o tiene distinta fecha de vencimiento, creamos un nuevo lote
                almacenItem.CantidadEnGramosOMl = cantidadNormalizada;
                almacenItem.FechaIngreso = DateTime.UtcNow;

                return await _almacenRepository.AgregarAsync(almacenItem);
            }
        }       
    }
}