using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChefEnCasa.Domain.Entities
{
    public class Receta
    {
        [Key]
        public int RecetaId { get; set; }
        public int SpoonacularId { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Resumen { get; set; } = string.Empty;
        public string Instrucciones { get; set; } = string.Empty;
        public string ImagenUrl { get; set; } = string.Empty;
        public int TiempoMinutos { get; set; }
        public int Porciones { get; set; }

        // --- NUEVO: Filtros de Dieta ---
        public bool EsVegetariano { get; set; }
        public bool EsVegano { get; set; }
        public bool EsSinGluten { get; set; }
        public bool EsSinLacteos { get; set; }

        // --- NUEVO: Macros por porción ---
        public int Calorias { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Carbohidratos { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Proteinas { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Grasas { get; set; }

        // Navegación
        public List<RecetaIngrediente> Ingredientes { get; set; } = new();
    }
}