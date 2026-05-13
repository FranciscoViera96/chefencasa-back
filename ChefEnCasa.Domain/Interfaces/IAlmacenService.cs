using ChefEnCasa.Domain.Entities;
using System.Threading.Tasks;

namespace ChefEnCasa.Domain.Interfaces
{
    public interface IAlmacenService
    {
        // Recibe la entidad y la información de la medida original
        Task<bool> AgregarOActualizarIngredienteAsync(Almacen almacenItem, decimal cantidadOriginal, string unidadOriginal);

    }
}