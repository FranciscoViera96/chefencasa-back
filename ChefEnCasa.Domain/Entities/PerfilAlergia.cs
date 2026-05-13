using System.ComponentModel.DataAnnotations;

namespace ChefEnCasa.Domain.Entities
{
    public class PerfilAlergia
    {
        [Key]
        public int PerfilAlergiaId { get; set; }

        public int PerfilSaludId { get; set; }
        public PerfilSalud PerfilSalud { get; set; }

        public int IngredienteId { get; set; }
        public Ingrediente Ingrediente { get; set; }
    }
}