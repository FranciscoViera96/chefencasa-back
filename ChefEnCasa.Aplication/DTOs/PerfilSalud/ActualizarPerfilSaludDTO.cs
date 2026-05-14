namespace ChefEnCasa.Application.DTOs
{
    public class ActualizarPerfilSaludDTO
    {
        public Guid UsuarioId { get; set; }
        public decimal Peso { get; set; }
        public decimal Altura { get; set; }
        public decimal IMC { get; set; }
        public int NecesidadCalorica { get; set; }
        public decimal TMB { get; set; }

        public bool EsVegetariano { get; set; }
        public bool EsVegano { get; set; }
        public bool EsCeliaco { get; set; }
        public bool IntoleranteLactosa { get; set; }

        // El front solo nos manda un array de IDs, ej: [5, 12, 45]
        public List<int> AlergiasIngredienteIds { get; set; } = new();
    }
}