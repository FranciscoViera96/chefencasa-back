namespace ChefEnCasa.Domain.Entities
{
    public class RecetaIngrediente
    {
        public int RecetaIngredienteId { get; set; }
        public int RecetaId { get; set; }
        public int IngredienteId { get; set; }
        public decimal Cantidad { get; set; }
        public string UnidadMedida { get; set; } = string.Empty;
        public decimal CantidadEnGramosOMl { get; set; } // Normalizado para facilitar la resta

        public Receta Receta { get; set; }
        public Ingrediente Ingrediente { get; set; }
    }
}