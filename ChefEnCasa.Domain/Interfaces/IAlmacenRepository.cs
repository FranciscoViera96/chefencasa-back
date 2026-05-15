using ChefEnCasa.Domain.Entities;

namespace ChefEnCasa.Domain.Interfaces
{
    public interface IAlmacenRepository
    {
        Task<List<Almacen>> ObtenerAlmacenPorUsuarioAsync(Guid usuarioId);
        Task<Almacen?> ObtenerItemEspecificoAsync(Guid usuarioId, int ingredienteId);
        Task<bool> AgregarAsync(Almacen almacen);
        Task<bool> ActualizarAsync(Almacen almacen);
        Task<Almacen?> ObtenerLoteEspecificoAsync(Guid usuarioId, int ingredienteId, DateTime? fechaCaducidad);
    }
}