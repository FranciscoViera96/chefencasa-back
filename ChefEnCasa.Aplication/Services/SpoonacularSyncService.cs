
//using ChefEnCasa.Application.DTOs.Spoonacular;
//using ChefEnCasa.Application.Interfaces;
//using ChefEnCasa.Domain.Constants;
//using ChefEnCasa.Domain.Entities;
//using ChefEnCasa.Infrastructure.Configurations;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using System.Text.Json;
//// Ya no necesitamos System.Text.RegularExpressions porque no limpiaremos HTML

//namespace ChefEnCasa.Application.Services
//{
//    public class SpoonacularSyncService : ISpoonacularSyncService
//    {
//        private readonly ChefEnCasaDbContext _context;
//        private readonly HttpClient _httpClient;
//        private readonly string _spoonApiKey;
//        private readonly string _deepLApiKey;

//        public SpoonacularSyncService(
//            ChefEnCasaDbContext context,
//            IConfiguration config,
//            HttpClient httpClient)
//        {
//            _context = context;
//            _httpClient = httpClient;

//            // Leemos las llaves desde appsettings.json para mayor seguridad
//            _spoonApiKey = config["Spoonacular:ApiKey"]
//                ?? throw new ArgumentException("Falta Spoonacular:ApiKey en appsettings.json");

//            _deepLApiKey = config["DeepL:ApiKey"]
//                ?? throw new ArgumentException("Falta DeepL:ApiKey en appsettings.json");
//        }

//        public async Task<int> SincronizarRecetasAsync(string letraBusqueda, int cantidadMaxima, int offset)
//        {
//            // 1. EL DISFRAZ: Evitamos ser detectados como bot básico de .NET
//            _httpClient.DefaultRequestHeaders.UserAgent.Clear();
//            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36");

//            // 2. LA URL MEJORADA: Agregamos instructionsRequired=true para no traer basura sin pasos
//            string url = $"https://api.spoonacular.com/recipes/complexSearch?apiKey={_spoonApiKey}&query={letraBusqueda}&number={cantidadMaxima}&offset={offset}&addRecipeInformation=true&fillIngredients=true&instructionsRequired=true";

//            var response = await _httpClient.GetAsync(url);

//            // 3. EL ESCUDO: Manejo de cuota agotada
//            if ((int)response.StatusCode == 402 || (int)response.StatusCode == 429)
//            {
//                throw new Exception("¡Límite diario agotado en Spoonacular! Intenta mañana o cambia la API Key.");
//            }

//            if (!response.IsSuccessStatusCode)
//                throw new Exception($"Error en Spoonacular: {response.StatusCode}");

//            var jsonString = await response.Content.ReadAsStringAsync();
//            var spoonacularData = JsonSerializer.Deserialize<SpoonacularResponseDTO>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

//            int recetasGuardadas = 0;
//            Random rnd = new Random();

//            if (spoonacularData?.Results == null) return 0;

//            foreach (var spRecipe in spoonacularData.Results)
//            {
//                // Evitar duplicados
//                if (await _context.Recetas.AnyAsync(r => r.SpoonacularId == spRecipe.Id)) continue;

//                // Pequeña pausa aleatoria para simular comportamiento humano y no saturar
//                await Task.Delay(rnd.Next(500, 1500));

//                // 4. PROCESAMIENTO Y TRADUCCIÓN
//                string rawInstrucciones = spRecipe.AnalyzedInstructions.Any()
//                    ? string.Join("\n", spRecipe.AnalyzedInstructions[0].Steps.Select(s => s.Step))
//                    : "";

//                // Traducimos SOLO lo importante (Ahorro masivo de caracteres)
//                string tituloEs = await TraducirConDeepLAsync(spRecipe.Title);
//                string instruccionesEs = await TraducirConDeepLAsync(rawInstrucciones);

//                // Texto genérico para no gastar DeepL en el SEO gringo
//                string resumenEs = "Deliciosa receta lista para preparar en casa.";

//                var nuevaReceta = new Receta
//                {
//                    SpoonacularId = spRecipe.Id,
//                    Titulo = tituloEs,
//                    Resumen = resumenEs,
//                    Instrucciones = instruccionesEs,
//                    ImagenUrl = spRecipe.Image,
//                    TiempoMinutos = spRecipe.ReadyInMinutes,
//                    Porciones = spRecipe.Servings,
//                    Ingredientes = new List<RecetaIngrediente>()
//                };

//                // 5. PROCESAR INGREDIENTES (Normalización)
//                foreach (var spIng in spRecipe.ExtendedIngredients)
//                {
//                    string nombreEs = (await TraducirConDeepLAsync(spIng.Name)).ToLower();

//                    // Buscar o crear en el catálogo maestro de ingredientes
//                    var ingredienteDb = await _context.Ingredientes
//                        .FirstOrDefaultAsync(i => i.NombreEspanol == nombreEs);

//                    if (ingredienteDb == null)
//                    {
//                        ingredienteDb = new Ingrediente
//                        {
//                            NombreOriginal = spIng.Name,
//                            NombreEspanol = nombreEs,
//                            ImagenUrl = ""
//                        };
//                        _context.Ingredientes.Add(ingredienteDb);
//                        await _context.SaveChangesAsync(); // Guardamos para tener el ID
//                    }

//                    // Normalización de medidas al sistema métrico (Gramos/ML)
//                    decimal normalizado;
//                    try { normalizado = ConversionesMedidas.ConvertirABase(spIng.Amount, spIng.Unit); }
//                    catch { normalizado = spIng.Amount; }

//                    nuevaReceta.Ingredientes.Add(new RecetaIngrediente
//                    {
//                        IngredienteId = ingredienteDb.IngredienteId,
//                        Cantidad = spIng.Amount,
//                        UnidadMedida = string.IsNullOrEmpty(spIng.Unit) ? "unidad" : spIng.Unit,
//                        CantidadEnGramosOMl = normalizado
//                    });
//                }

//                _context.Recetas.Add(nuevaReceta);
//                await _context.SaveChangesAsync();
//                recetasGuardadas++;
//            }

//            return recetasGuardadas;
//        }

//        private async Task<string> TraducirConDeepLAsync(string texto)
//        {
//            if (string.IsNullOrWhiteSpace(texto)) return "";

//            string url = "https://api-free.deepl.com/v2/translate";

//            // 1. Armamos la petición de forma manual para inyectar el Header
//            using var request = new HttpRequestMessage(HttpMethod.Post, url);

//            // 2. EL ARREGLO: Pasamos la llave por el Header como exige DeepL ahora
//            request.Headers.Add("Authorization", $"DeepL-Auth-Key {_deepLApiKey}");

//            // 3. Ya no mandamos la auth_key en el body, solo el texto y el idioma
//            var content = new FormUrlEncodedContent(new[]
//            {
//                new KeyValuePair<string, string>("text", texto),
//                new KeyValuePair<string, string>("target_lang", "ES")
//            });

//            request.Content = content;

//            try
//            {
//                // Usamos SendAsync en lugar de PostAsync porque armamos un HttpRequestMessage
//                var response = await _httpClient.SendAsync(request);

//                if (!response.IsSuccessStatusCode) 
//                {
//                    var errorBody = await response.Content.ReadAsStringAsync();
//                    Console.WriteLine($"\n--- [ERROR DE DEEPL] ---");
//                    Console.WriteLine($"Status: {response.StatusCode}");
//                    Console.WriteLine($"Detalle: {errorBody}");
//                    Console.WriteLine($"------------------------\n");
//                    return texto; // Fallback
//                }

//                var jsonResponse = await response.Content.ReadAsStringAsync();
//                using var doc = JsonDocument.Parse(jsonResponse);
//                return doc.RootElement.GetProperty("translations")[0].GetProperty("text").GetString() ?? texto;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"\n--- [EXCEPCIÓN DEEPL] ---\n{ex.Message}\n--------------------------\n");
//                return texto; 
//            }
//        }
//    }
//}
using ChefEnCasa.Application.DTOs.Spoonacular;
using ChefEnCasa.Application.Interfaces;
using ChefEnCasa.Domain.Constants;
using ChefEnCasa.Domain.Entities;
using ChefEnCasa.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace ChefEnCasa.Application.Services
{
    public class SpoonacularSyncService : ISpoonacularSyncService
    {
        private readonly ChefEnCasaDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly string _spoonApiKey;
        private readonly string _deepLApiKey;

        public SpoonacularSyncService(
            ChefEnCasaDbContext context,
            IConfiguration config,
            HttpClient httpClient)
        {
            _context = context;
            _httpClient = httpClient;
            _spoonApiKey = config["Spoonacular:ApiKey"] ?? throw new ArgumentException("Falta Spoonacular:ApiKey");
            _deepLApiKey = config["DeepL:ApiKey"] ?? throw new ArgumentException("Falta DeepL:ApiKey");
        }

        public async Task<int> SincronizarRecetasAsync(string letraBusqueda, int cantidadMaxima, int offset)
        {
            _httpClient.DefaultRequestHeaders.UserAgent.Clear();
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36");

            // NUEVO: addRecipeNutrition=true
            string url = $"https://api.spoonacular.com/recipes/complexSearch?apiKey={_spoonApiKey}&query={letraBusqueda}&number={cantidadMaxima}&offset={offset}&addRecipeInformation=true&fillIngredients=true&instructionsRequired=true&addRecipeNutrition=true";

            var response = await _httpClient.GetAsync(url);

            if ((int)response.StatusCode == 402 || (int)response.StatusCode == 429)
                throw new Exception("¡Límite diario agotado en Spoonacular!");

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Error en Spoonacular: {response.StatusCode}");

            var jsonString = await response.Content.ReadAsStringAsync();
            var spoonacularData = JsonSerializer.Deserialize<SpoonacularResponseDTO>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            int recetasGuardadas = 0;
            Random rnd = new Random();

            if (spoonacularData?.Results == null) return 0;

            foreach (var spRecipe in spoonacularData.Results)
            {
                if (await _context.Recetas.AnyAsync(r => r.SpoonacularId == spRecipe.Id)) continue;

                await Task.Delay(rnd.Next(500, 1500));

                string rawInstrucciones = spRecipe.AnalyzedInstructions.Any()
                    ? string.Join("\n", spRecipe.AnalyzedInstructions[0].Steps.Select(s => s.Step))
                    : "";

                string tituloEs = await TraducirConDeepLAsync(spRecipe.Title);
                string instruccionesEs = await TraducirConDeepLAsync(rawInstrucciones);
                string resumenEs = "Deliciosa receta lista para preparar en casa.";

                // --- NUEVO: Extraer Nutrición ---
                int calorias = 0;
                decimal carbs = 0, proteinas = 0, grasas = 0;

                if (spRecipe.Nutrition != null && spRecipe.Nutrition.Nutrients.Any())
                {
                    var nut = spRecipe.Nutrition.Nutrients;
                    calorias = (int)(nut.FirstOrDefault(n => n.Name == "Calories")?.Amount ?? 0);
                    carbs = nut.FirstOrDefault(n => n.Name == "Carbohydrates")?.Amount ?? 0;
                    proteinas = nut.FirstOrDefault(n => n.Name == "Protein")?.Amount ?? 0;
                    grasas = nut.FirstOrDefault(n => n.Name == "Fat")?.Amount ?? 0;
                }

                var nuevaReceta = new Receta
                {
                    SpoonacularId = spRecipe.Id,
                    Titulo = tituloEs,
                    Resumen = resumenEs,
                    Instrucciones = instruccionesEs,
                    ImagenUrl = spRecipe.Image,
                    TiempoMinutos = spRecipe.ReadyInMinutes,
                    Porciones = spRecipe.Servings,

                    // Mapeo de Dietas
                    EsVegetariano = spRecipe.Vegetarian,
                    EsVegano = spRecipe.Vegan,
                    EsSinGluten = spRecipe.GlutenFree,
                    EsSinLacteos = spRecipe.DairyFree,

                    // Mapeo Nutricional
                    Calorias = calorias,
                    Carbohidratos = carbs,
                    Proteinas = proteinas,
                    Grasas = grasas,

                    Ingredientes = new List<RecetaIngrediente>()
                };

                foreach (var spIng in spRecipe.ExtendedIngredients)
                {
                    string nombreEs = (await TraducirConDeepLAsync(spIng.Name)).ToLower();

                    var ingredienteDb = await _context.Ingredientes
                        .FirstOrDefaultAsync(i => i.NombreEspanol == nombreEs);

                    if (ingredienteDb == null)
                    {
                        // 1. ARREGLO DE IMAGEN: Armamos la URL completa
                        string urlImagenCompleta = string.IsNullOrEmpty(spIng.Image)
                            ? ""
                            : $"https://spoonacular.com/cdn/ingredients_100x100/{spIng.Image}";

                        // 2. CATEGORÍA Y VIDA ÚTIL: Calculamos en base al pasillo (Aisle)
                        var infoCategoria = ProcesarCategoriaYVidaUtil(spIng.Aisle);

                        ingredienteDb = new Ingrediente
                        {
                            NombreOriginal = spIng.Name,
                            NombreEspanol = nombreEs,
                            ImagenUrl = urlImagenCompleta,
                            Categoria = infoCategoria.NombreCategoria,
                            DiasVidaUtilEstimada = infoCategoria.DiasEstimados
                        };
                        _context.Ingredientes.Add(ingredienteDb);
                        await _context.SaveChangesAsync();
                    }

                    decimal normalizado;
                    try { normalizado = ConversionesMedidas.ConvertirABase(spIng.Amount, spIng.Unit); }
                    catch { normalizado = spIng.Amount; }

                    nuevaReceta.Ingredientes.Add(new RecetaIngrediente
                    {
                        IngredienteId = ingredienteDb.IngredienteId,
                        Cantidad = spIng.Amount,
                        UnidadMedida = TraducirUnidadEstatica(spIng.Unit),
                        CantidadEnGramosOMl = normalizado
                    });
                }

                _context.Recetas.Add(nuevaReceta);
                await _context.SaveChangesAsync();
                recetasGuardadas++;
            }

            return recetasGuardadas;
        }

        private async Task<string> TraducirConDeepLAsync(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return "";

            string url = "https://api-free.deepl.com/v2/translate";

            using var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("Authorization", $"DeepL-Auth-Key {_deepLApiKey}");

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("text", texto),
                new KeyValuePair<string, string>("target_lang", "ES")
            });

            request.Content = content;

            try
            {
                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"\n--- [ERROR DE DEEPL] ---\nStatus: {response.StatusCode}\nDetalle: {errorBody}\n------------------------\n");
                    return texto;
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(jsonResponse);
                return doc.RootElement.GetProperty("translations")[0].GetProperty("text").GetString() ?? texto;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n--- [EXCEPCIÓN DEEPL] ---\n{ex.Message}\n--------------------------\n");
                return texto;
            }
        }

        // --- NUEVO: Traductor Gratuito de Unidades ---
        private string TraducirUnidadEstatica(string unitIngles)
        {
            if (string.IsNullOrWhiteSpace(unitIngles)) return "unidad";

            return unitIngles.ToLower() switch
            {
                "cup" or "cups" => "taza",
                "tablespoon" or "tablespoons" or "tbsp" => "cda",
                "teaspoon" or "teaspoons" or "tsp" => "cdta",
                "ounce" or "ounces" or "oz" => "oz",
                "clove" or "cloves" => "diente",
                "pound" or "pounds" or "lb" => "lb",
                "large" or "medium" or "small" => "unidad",
                "servings" or "serving" => "porción",
                "pinch" or "pinches" => "pizca",
                "handful" => "puñado",
                "slice" or "slices" => "rebanada",
                "g" or "grams" => "g",
                "ml" or "milliliters" => "ml",
                _ => unitIngles
            };
        }

        // --- NUEVO: Clasificador Gratuito de Categorías ---
        private (string NombreCategoria, int? DiasEstimados) ProcesarCategoriaYVidaUtil(string aisleIngles)
        {
            if (string.IsNullOrWhiteSpace(aisleIngles)) return ("Otros", null);

            return aisleIngles.ToLower() switch
            {
                var a when a.Contains("produce") || a.Contains("fruit") || a.Contains("vegetable") => ("Frutas y Verduras", 10),
                var a when a.Contains("meat") || a.Contains("poultry") => ("Carnes", 3),
                var a when a.Contains("seafood") || a.Contains("fish") => ("Pescados y Mariscos", 2),
                var a when a.Contains("milk") || a.Contains("dairy") || a.Contains("cheese") => ("Lácteos", 14),
                var a when a.Contains("bakery") || a.Contains("bread") => ("Panadería", 5),
                var a when a.Contains("spices") || a.Contains("seasoning") => ("Especias y Condimentos", 365), // 1 año
                var a when a.Contains("canned") || a.Contains("jarred") => ("Enlatados y Conservas", 365),
                var a when a.Contains("pasta") || a.Contains("rice") || a.Contains("grains") => ("Despensa Seca", 365),
                var a when a.Contains("condiment") || a.Contains("sauce") => ("Salsas y Condimentos", 90),
                var a when a.Contains("nut") || a.Contains("seed") => ("Frutos Secos y Semillas", 180),
                var a when a.Contains("frozen") => ("Congelados", 180),
                _ => ("Otros", null) // Si no sabemos qué es, obligamos a que el usuario ponga la fecha
            };
        }
    }
}