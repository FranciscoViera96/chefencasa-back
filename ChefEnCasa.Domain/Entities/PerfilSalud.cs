using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChefEnCasa.Domain.Entities
{
    public class PerfilSalud
    {
        [Key]
        public int PerfilSaludId { get; set; }
        public Guid UsuarioId { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal Peso { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal Altura { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal IMC { get; set; }

        // --- LOS NUEVOS ESPEJOS DE LA RECETA ---
        public bool EsVegetariano { get; set; }
        public bool EsVegano { get; set; }
        public bool EsCeliaco { get; set; } // Hace match con EsSinGluten de la receta
        public bool IntoleranteLactosa { get; set; } // Hace match con EsSinLacteos de la receta

        // Calorías
        public int NecesidadCalorica { get; set; }

        [Column(TypeName = "decimal(8,2)")]
        public decimal TMB { get; set; }
        public DateTime UltimaActualizacion { get; set; } = DateTime.UtcNow;

        // Navegación
        public Usuario Usuario { get; set; }

        // --- NUEVA RELACIÓN PARA ALERGIAS ESPECÍFICAS ---
        public List<PerfilAlergia> Alergias { get; set; } = new();
    }
}