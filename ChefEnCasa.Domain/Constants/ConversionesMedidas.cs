namespace ChefEnCasa.Domain.Constants
{
    //public static class ConversionesMedidas
    //{
    //    public static readonly Dictionary<string, decimal> Factores = new(StringComparer.OrdinalIgnoreCase)
    //    {
    //        { "gram", 1m },
    //        { "ml", 1m },
    //        { "kg", 1000m },
    //        { "l", 1000m },
    //        { "tbsp", 15m },
    //        { "tsp", 5m },
    //        { "cup", 240m },
    //        { "oz", 28.35m },
    //        { "lb", 453.592m },
    //        { "pinche", 0.36m },
    //        { "clove", 5m },
    //        { "head", 1000m },
    //        { "ounce", 28.35m },
    //        { "serving", 0.5m },
    //        { "strip", 5m },
    //        { "large", 100m },
    //        { "unidad", 100m }
    //    };

    //    public static decimal ConvertirABase(decimal cantidad, string unidad)
    //    {
    //        if (string.IsNullOrWhiteSpace(unidad))
    //            unidad = "gram";

    //        if (unidad.EndsWith("s", StringComparison.OrdinalIgnoreCase))
    //            unidad = unidad.Substring(0, unidad.Length - 1);

    //        if (Factores.TryGetValue(unidad, out decimal factor))
    //        {
    //            return cantidad * factor;
    //        }

    //        throw new ArgumentException($"Unidad desconocida: {unidad}");
    //    }
    //}
    public static class ConversionesMedidas
    {
        public static decimal ConvertirABase(decimal cantidad, string unidad)
        {
            if (string.IsNullOrWhiteSpace(unidad)) return cantidad;

            var u = unidad.ToLower().Trim();

            // 1. Unidades de Masa (Gramos)
            if (u == "g" || u == "gram" || u == "grams" || u == "gramo" || u == "gramos") return cantidad;
            if (u == "kg" || u == "kilogram" || u == "kilograms" || u == "kilo" || u == "kilos") return cantidad * 1000;
            if (u == "oz" || u == "ounce" || u == "ounces" || u == "onza" || u == "onzas") return cantidad * 28.35m;
            if (u == "lb" || u == "pound" || u == "pounds" || u == "libra" || u == "libras") return cantidad * 453.59m;

            // 2. Unidades de Volumen (Mililitros)
            if (u == "ml" || u == "milliliter" || u == "mililitro" || u == "mililitros") return cantidad;
            if (u == "l" || u == "liter" || u == "liters" || u == "litro" || u == "litros") return cantidad * 1000;

            // 3. Unidades Subjetivas / Cucharas / Tazas (Promedios)
            if (u == "cda" || u == "tbsp" || u == "tablespoon" || u == "cucharada" || u == "cucharadas") return cantidad * 15; // 1 cda = ~15g/ml
            if (u == "cdta" || u == "tsp" || u == "teaspoon" || u == "cucharadita" || u == "cucharaditas") return cantidad * 5;  // 1 cdta = ~5g/ml
            if (u == "taza" || u == "cup" || u == "cups" || u == "tazas") return cantidad * 240; // 1 taza = ~240g/ml
            if (u == "pizca" || u == "pinch") return cantidad * 1; // 1 pizca = ~1g

            // 4. Unidades Físicas (Aproximaciones al ojo)
            if (u == "diente" || u == "clove" || u == "cloves" || u == "dientes") return cantidad * 5; // 1 diente de ajo = 5g
            if (u == "bunch" || u == "manojo") return cantidad * 150; // 1 manojo de cilantro/col = 150g
            if (u == "porción" || u == "serving" || u == "servings") return cantidad * 100; // 1 porción genérica = 100g
            if (u == "unidad" || u == "un" || u == "piece" || u == "pieces") return cantidad * 150; // 1 cebolla/tomate promedio = 150g

            // Si no la reconoce, asume que es la cantidad original para no dejar en 0
            return cantidad;
        }
    }
}