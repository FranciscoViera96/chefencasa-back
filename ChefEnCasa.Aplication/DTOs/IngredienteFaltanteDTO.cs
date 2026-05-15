using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChefEnCasa.Aplication.DTOs
{
    public class IngredienteFaltanteDTO
    {
        public int IngredienteId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal CantidadFaltanteGramos { get; set; }
    }
}
