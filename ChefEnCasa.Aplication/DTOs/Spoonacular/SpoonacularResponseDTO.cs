//using System.Text.Json.Serialization;

//namespace ChefEnCasa.Application.DTOs.Spoonacular
//{
//    public class SpoonacularResponseDTO
//    {
//        [JsonPropertyName("results")]
//        public List<SpoonacularRecipeDTO> Results { get; set; } = new();
//    }

//    public class SpoonacularRecipeDTO
//    {
//        public int Id { get; set; }
//        public string Title { get; set; } = string.Empty;
//        public string Summary { get; set; } = string.Empty;
//        public string Image { get; set; } = string.Empty;
//        public int ReadyInMinutes { get; set; }
//        public int Servings { get; set; }

//        [JsonPropertyName("extendedIngredients")]
//        public List<SpoonacularIngredientDTO> ExtendedIngredients { get; set; } = new();

//        [JsonPropertyName("analyzedInstructions")]
//        public List<SpoonacularInstructionDTO> AnalyzedInstructions { get; set; } = new();
//    }

//    public class SpoonacularIngredientDTO
//    {
//        public string Name { get; set; } = string.Empty;
//        public decimal Amount { get; set; }
//        public string Unit { get; set; } = string.Empty;
//    }

//    public class SpoonacularInstructionDTO
//    {
//        public List<SpoonacularStepDTO> Steps { get; set; } = new();
//    }

//    public class SpoonacularStepDTO
//    {
//        public string Step { get; set; } = string.Empty;
//    }
//}
using System.Text.Json.Serialization;

namespace ChefEnCasa.Application.DTOs.Spoonacular
{
    public class SpoonacularResponseDTO
    {
        [JsonPropertyName("results")]
        public List<SpoonacularRecipeDTO> Results { get; set; } = new();
    }

    public class SpoonacularRecipeDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public int ReadyInMinutes { get; set; }
        public int Servings { get; set; }

        // --- NUEVO: Dietas y Alergias ---
        public bool Vegetarian { get; set; }
        public bool Vegan { get; set; }
        public bool GlutenFree { get; set; }
        public bool DairyFree { get; set; }

        // --- NUEVO: Nutrición ---
        public SpoonacularNutritionDTO Nutrition { get; set; }

        [JsonPropertyName("extendedIngredients")]
        public List<SpoonacularIngredientDTO> ExtendedIngredients { get; set; } = new();

        [JsonPropertyName("analyzedInstructions")]
        public List<SpoonacularInstructionDTO> AnalyzedInstructions { get; set; } = new();
    }

    public class SpoonacularNutritionDTO
    {
        public List<SpoonacularNutrientDTO> Nutrients { get; set; } = new();
    }

    public class SpoonacularNutrientDTO
    {
        public string Name { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Unit { get; set; } = string.Empty;
    }

    public class SpoonacularIngredientDTO
    {
        public string Name { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Unit { get; set; } = string.Empty;

        // --- NUEVOS CAMPOS PARA INGREDIENTES ---
        public string Image { get; set; } = string.Empty;
        public string Aisle { get; set; } = string.Empty;
    }

    public class SpoonacularInstructionDTO
    {
        public List<SpoonacularStepDTO> Steps { get; set; } = new();
    }

    public class SpoonacularStepDTO
    {
        public string Step { get; set; } = string.Empty;
    }
}