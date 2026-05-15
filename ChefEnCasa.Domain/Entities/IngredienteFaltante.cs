namespace ChefEnCasa.Domain.Entities
{
    public class IngredienteFaltante
    {
        public int IngredienteId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal CantidadFaltanteGramos { get; set; }
    }
}