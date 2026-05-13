using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChefEnCasa.Domain.Entities
{
    public class ListaCompraDetalle
    {
        [Key]
        public int ListaCompraDetalleId { get; set; }

        public int ListaCompraId { get; set; }
        public ListaCompra ListaCompra { get; set; }

        public int IngredienteId { get; set; }
        public Ingrediente Ingrediente { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal CantidadFaltante { get; set; }
        public string UnidadMedidaSugerida { get; set; } = string.Empty;

        public bool Comprado { get; set; } = false; // El famoso "Check" del supermercado
    }
}