using System.ComponentModel.DataAnnotations;

namespace ChefEnCasa.Domain.Entities
{
    public class ListaCompra
    {
        [Key]
        public int ListaCompraId { get; set; }
        public Guid UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public bool EstaCompletada { get; set; } = false;

        public List<ListaCompraDetalle> Detalles { get; set; } = new();
    }
}