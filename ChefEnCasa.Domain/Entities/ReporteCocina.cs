namespace ChefEnCasa.Domain.Entities
{
    public class ReporteCocina
    {
        public string Mensaje { get; set; } = string.Empty;
        public int PuntosObtenidos { get; set; }
        public List<DetalleConsumo> ResumenInventario { get; set; } = new();
    }

    public class DetalleConsumo
    {
        public string Ingrediente { get; set; } = string.Empty;
        public decimal CantidadRequeridaGramos { get; set; }
        public decimal StockAnteriorGramos { get; set; }
        public decimal StockRestanteGramos { get; set; }
        public int LotesConsumidos { get; set; }
    }
}