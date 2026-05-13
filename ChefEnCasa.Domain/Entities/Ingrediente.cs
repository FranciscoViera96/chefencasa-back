using System.ComponentModel.DataAnnotations;

namespace ChefEnCasa.Domain.Entities
{
    public class Ingrediente
    {
        [Key]
        public int IngredienteId { get; set; }
        public string NombreOriginal { get; set; } = string.Empty;
        public string NombreEspanol { get; set; } = string.Empty;
        public string ImagenUrl { get; set; } = string.Empty;

        // --- NUEVOS CAMPOS ---
        public string Categoria { get; set; } = string.Empty;
        public int? DiasVidaUtilEstimada { get; set; } // Puede ser null si es un embutido que requiere fecha manual

        //public ICollection<Almacen> Almacenes { get; set; } = new List<Almacen>();
    }
}