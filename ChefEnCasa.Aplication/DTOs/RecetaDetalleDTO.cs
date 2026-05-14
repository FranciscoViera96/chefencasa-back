namespace ChefEnCasa.Application.DTOs
{
    public class RecetaDetalleDTO
    {
        public int RecetaId { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Resumen { get; set; } = string.Empty;
        public string Instrucciones { get; set; } = string.Empty;
        public string ImagenUrl { get; set; } = string.Empty;
        public int TiempoMinutos { get; set; }
        public int Porciones { get; set; }
        public int Calorias { get; set; }
        public decimal Carbohidratos { get; set; }
        public decimal Proteinas { get; set; }
        public decimal Grasas { get; set; }
        public bool EsVegetariano { get; set; }
        public bool EsVegano { get; set; }
        public bool EsSinGluten { get; set; }
        public bool EsSinLacteos { get; set; }

        public List<RecetaIngredienteDTO> Ingredientes { get; set; } = new();
    }

    public class RecetaIngredienteDTO
    {
        public int IngredienteId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string ImagenUrl { get; set; } = string.Empty;
        public decimal Cantidad { get; set; }
        public string UnidadMedida { get; set; } = string.Empty;
    }
}