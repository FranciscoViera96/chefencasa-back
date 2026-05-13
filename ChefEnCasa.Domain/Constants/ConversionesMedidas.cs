namespace ChefEnCasa.Domain.Constants
{
    public static class ConversionesMedidas
    {
        public static readonly Dictionary<string, decimal> Factores = new(StringComparer.OrdinalIgnoreCase)
        {
            { "gram", 1m },
            { "ml", 1m },
            { "kg", 1000m },
            { "l", 1000m },
            { "tbsp", 15m },
            { "tsp", 5m },
            { "cup", 240m },
            { "oz", 28.35m },
            { "lb", 453.592m },
            { "pinche", 0.36m },
            { "clove", 5m },
            { "head", 1000m },
            { "ounce", 28.35m },
            { "serving", 0.5m },
            { "strip", 5m },
            { "large", 100m },
            { "unidad", 100m }
        };

        public static decimal ConvertirABase(decimal cantidad, string unidad)
        {
            if (string.IsNullOrWhiteSpace(unidad))
                unidad = "gram";

            if (unidad.EndsWith("s", StringComparison.OrdinalIgnoreCase))
                unidad = unidad.Substring(0, unidad.Length - 1);

            if (Factores.TryGetValue(unidad, out decimal factor))
            {
                return cantidad * factor;
            }

            throw new ArgumentException($"Unidad desconocida: {unidad}");
        }
    }
}