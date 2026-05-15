using ChefEnCasa.Domain.Entities;
using ChefEnCasa.Domain.Interfaces;

namespace ChefEnCasa.Application.Services
{
    public class RecetaService(
        IRecetaRepository recetaRepository,
        IAlmacenRepository almacenRepository,
        IUsuarioRepository usuarioRepository) : IRecetaService
    {
        private readonly IRecetaRepository _recetaRepository = recetaRepository;
        private readonly IAlmacenRepository _almacenRepository = almacenRepository;
        private readonly IUsuarioRepository _usuarioRepository = usuarioRepository;

        public async Task<(List<Receta> Recetas, int TotalRegistros)> ObtenerRecetasPaginadasAsync(
            int pagina, int tamañoPagina, string? busqueda, Guid? usuarioId = null)
        {
            return await _recetaRepository.ObtenerRecetasPaginadasAsync(pagina, tamañoPagina, busqueda, usuarioId);
        }

        public async Task<Receta?> ObtenerRecetaPorIdAsync(int recetaId)
        {
            return await _recetaRepository.ObtenerRecetaConIngredientesAsync(recetaId);
        }

        public async Task<ReporteCocina> CocinarRecetaAsync(Guid usuarioId, int recetaId)
        {
            // 1. Validaciones iniciales
            var receta = await _recetaRepository.ObtenerRecetaConIngredientesAsync(recetaId)
                ?? throw new InvalidOperationException("La receta no existe.");

            var idsRequeridos = receta.Ingredientes.Select(ri => ri.IngredienteId).ToList();
            var inventarioUsuario = await _almacenRepository.ObtenerLotesParaIngredientesAsync(usuarioId, idsRequeridos);

            var lotesAActualizar = new List<Almacen>();
            var lotesAEliminar = new List<Almacen>();

            // Inicializamos el reporte de dominio
            var respuesta = new ReporteCocina
            {
                Mensaje = $"¡Receta '{receta.Titulo}' preparada con éxito!",
                PuntosObtenidos = 10
            };

            // 2. Aplicación del Algoritmo FEFO y llenado del reporte
            foreach (var requerimiento in receta.Ingredientes)
            {
                decimal cantidadNecesaria = requerimiento.CantidadEnGramosOMl;
                decimal cantidadRequeridaOriginal = cantidadNecesaria;

                // Ordenamos por fecha de caducidad (FEFO)
                var lotesDisponibles = inventarioUsuario
                    .Where(a => a.IngredienteId == requerimiento.IngredienteId)
                    .OrderBy(a => a.FechaCaducidad.HasValue ? 0 : 1)
                    .ThenBy(a => a.FechaCaducidad)
                    .ToList();

                decimal stockInicial = lotesDisponibles.Sum(a => a.CantidadEnGramosOMl);

                // Validación de seguridad
                if (stockInicial < cantidadNecesaria)
                {
                    throw new InvalidOperationException($"Stock insuficiente de '{requerimiento.Ingrediente.NombreEspanol}'. Requieres {cantidadNecesaria}g y tienes {stockInicial}g.");
                }

                int lotesAfectados = 0;

                foreach (var lote in lotesDisponibles)
                {
                    if (cantidadNecesaria <= 0) break;

                    lotesAfectados++;

                    if (lote.CantidadEnGramosOMl <= cantidadNecesaria)
                    {
                        cantidadNecesaria -= lote.CantidadEnGramosOMl;
                        lotesAEliminar.Add(lote);
                    }
                    else
                    {
                        lote.CantidadEnGramosOMl -= cantidadNecesaria;
                        cantidadNecesaria = 0;
                        lotesAActualizar.Add(lote);
                    }
                }

                // Agregamos la trazabilidad de este ingrediente al reporte
                respuesta.ResumenInventario.Add(new DetalleConsumo
                {
                    Ingrediente = requerimiento.Ingrediente.NombreEspanol,
                    CantidadRequeridaGramos = cantidadRequeridaOriginal,
                    StockAnteriorGramos = stockInicial,
                    StockRestanteGramos = stockInicial - cantidadRequeridaOriginal,
                    LotesConsumidos = lotesAfectados
                });
            }

            // 3. Persistencia de cambios
            await _almacenRepository.AplicarDescuentoFEFOAsync(lotesAActualizar, lotesAEliminar);

            // 4. Gamificación
            var usuario = await _usuarioRepository.GetByIdAsync(usuarioId);
            if (usuario != null)
            {
                usuario.Puntos += respuesta.PuntosObtenidos;
                await _usuarioRepository.UpdateUserAsync(usuario);
            }

            return respuesta;
        }

        public async Task<List<IngredienteFaltante>> ObtenerFaltantesParaRecetaAsync(Guid usuarioId, int recetaId)
        {
            var receta = await _recetaRepository.ObtenerRecetaConIngredientesAsync(recetaId)
                ?? throw new InvalidOperationException("La receta no existe.");

            var idsRequeridos = receta.Ingredientes.Select(ri => ri.IngredienteId).ToList();
            var inventarioUsuario = await _almacenRepository.ObtenerLotesParaIngredientesAsync(usuarioId, idsRequeridos);

            var faltantes = new List<IngredienteFaltante>();

            foreach (var requerimiento in receta.Ingredientes)
            {
                // Sumamos todo el stock del usuario para este ingrediente, sin importar la fecha
                decimal stockTotal = inventarioUsuario
                    .Where(a => a.IngredienteId == requerimiento.IngredienteId)
                    .Sum(a => a.CantidadEnGramosOMl);

                decimal cantidadNecesaria = requerimiento.CantidadEnGramosOMl;

                // Si le falta, lo agregamos a la lista de "mandados"
                if (stockTotal < cantidadNecesaria)
                {
                    faltantes.Add(new IngredienteFaltante
                    {
                        IngredienteId = requerimiento.IngredienteId,
                        Nombre = requerimiento.Ingrediente.NombreEspanol,
                        CantidadFaltanteGramos = cantidadNecesaria - stockTotal
                    });
                }
            }

            return faltantes;
        }
    }
}