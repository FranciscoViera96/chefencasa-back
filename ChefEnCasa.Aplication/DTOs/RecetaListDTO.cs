namespace ChefEnCasa.Application.DTOs
{
    public class RecetaListDTO
    {
        public int RecetaId { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string ImagenUrl { get; set; } = string.Empty;
        public int TiempoMinutos { get; set; }
        public int Calorias { get; set; }
        public bool EsVegano { get; set; }
        public bool EsVegetariano { get; set; }
    }
}