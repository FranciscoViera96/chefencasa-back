using Microsoft.EntityFrameworkCore; // ¡Este es el bueno!

namespace ChefEnCasa.Domain.Entities
{
    public class Almacen
    {
        public Guid AlmacenId { get; set; } = Guid.NewGuid();
        public Guid UsuarioId { get; set; }
        public int IngredienteId { get; set; }

        [Precision(18, 2)]
        public decimal CantidadEnGramosOMl { get; set; }
        public DateTime FechaIngreso { get; set; }
        public bool EsPerecedero { get; set; }
        public DateTime? FechaCaducidad { get; set; } // Fundamental para el sistema de alertas

        public Usuario Usuario { get; set; }
        public Ingrediente Ingrediente { get; set; }
    }
}