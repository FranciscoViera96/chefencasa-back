namespace ChefEnCasa.Application.DTOs
{
    public class ResultadoCocinaDTO
    {
        public string Mensaje { get; set; } = string.Empty;
        public int PuntosObtenidos { get; set; }
        public List<DetalleConsumoDTO> ResumenInventario { get; set; } = new();
    }

    public class DetalleConsumoDTO
    {
        public string Ingrediente { get; set; } = string.Empty;
        public decimal CantidadRequeridaGramos { get; set; }
        public decimal StockAnteriorGramos { get; set; }
        public decimal StockRestanteGramos { get; set; }
        public int LotesConsumidos { get; set; }
    }
}