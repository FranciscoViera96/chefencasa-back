namespace ChefEnCasa.Application.DTOs
{
    public class AgregarAlmacenDTO
    {
        public Guid UsuarioId { get; set; }
        public int IngredienteId { get; set; }
        public decimal Cantidad { get; set; }
        public string UnidadDeMedida { get; set; } = string.Empty; // ej: "cup", "gram", "tbsp"
        public bool EsPerecedero { get; set; }
        public DateTime? FechaCaducidad { get; set; }
    }
}