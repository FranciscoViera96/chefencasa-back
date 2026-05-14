namespace ChefEnCasa.Application.DTOs
{
    public class AlmacenItemDTO
    {
        public Guid AlmacenId { get; set; }
        public int IngredienteId { get; set; }
        public string NombreIngrediente { get; set; } = string.Empty;
        public string ImagenUrl { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public decimal CantidadEnGramosOMl { get; set; }
        public bool EsPerecedero { get; set; }
        public DateTime? FechaCaducidad { get; set; }

        // Propiedad calculada útil para el front: ¿Está vencido?
        public bool EstaVencido => FechaCaducidad.HasValue && FechaCaducidad.Value < DateTime.UtcNow;
    }
}