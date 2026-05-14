namespace ChefEnCasa.Application.DTOs
{
    public class PerfilSaludDTO
    {
        public decimal Peso { get; set; }
        public decimal Altura { get; set; }
        public decimal IMC { get; set; }
        public int NecesidadCalorica { get; set; }
        public decimal TMB { get; set; }

        public bool EsVegetariano { get; set; }
        public bool EsVegano { get; set; }
        public bool EsCeliaco { get; set; }
        public bool IntoleranteLactosa { get; set; }

        // Mandamos los nombres de las alergias para que el front las muestre en etiquetas
        public List<AlergiaItemDTO> Alergias { get; set; } = new();
    }

    public class AlergiaItemDTO
    {
        public int IngredienteId { get; set; }
        public string NombreIngrediente { get; set; } = string.Empty;
    }
}