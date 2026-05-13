using System.ComponentModel.DataAnnotations;

namespace ChefEnCasa.Domain.Entities
{
    public class RecetaFavorita
    {
        [Key]
        public int RecetaFavoritaId { get; set; }

        public Guid UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        public int RecetaId { get; set; }
        public Receta Receta { get; set; }

        public DateTime FechaGuardado { get; set; } = DateTime.UtcNow;
    }
}