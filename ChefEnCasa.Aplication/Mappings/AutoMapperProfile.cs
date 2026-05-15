using AutoMapper;
using ChefEnCasa.Aplication.DTOs;
using ChefEnCasa.Application.DTOs;
using ChefEnCasa.Domain.Entities;

namespace ChefEnCasa.Application.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Usuario (Lo que ya tenías)
            CreateMap<Usuario, UsuarioDTO>();
            CreateMap<UsuarioCreateDTO, Usuario>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.UsuarioId, opt => opt.Ignore());

            // --- NUEVO: Recetas ---
            CreateMap<Receta, RecetaListDTO>();

            CreateMap<Receta, RecetaDetalleDTO>();

            // Mapeo especial para los ingredientes de la receta
            CreateMap<RecetaIngrediente, RecetaIngredienteDTO>()
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Ingrediente.NombreEspanol))
                .ForMember(dest => dest.ImagenUrl, opt => opt.MapFrom(src => src.Ingrediente.ImagenUrl));

            // Mapeo del Almacén para listar (aplanando datos del Ingrediente)
            CreateMap<Almacen, AlmacenItemDTO>()
                .ForMember(dest => dest.NombreIngrediente, opt => opt.MapFrom(src => src.Ingrediente.NombreEspanol))
                .ForMember(dest => dest.ImagenUrl, opt => opt.MapFrom(src => src.Ingrediente.ImagenUrl))
                .ForMember(dest => dest.Categoria, opt => opt.MapFrom(src => src.Ingrediente.Categoria));

            // Perfil de Salud
            CreateMap<PerfilSalud, PerfilSaludDTO>();
            CreateMap<PerfilAlergia, AlergiaItemDTO>()
                .ForMember(dest => dest.NombreIngrediente, opt => opt.MapFrom(src => src.Ingrediente.NombreEspanol));
            CreateMap<ActualizarPerfilSaludDTO, PerfilSalud>();



            // 1. Mapear el reporte principal
            CreateMap<ReporteCocina, ResultadoCocinaDTO>();

            // 2. Mapear el detalle de cada ingrediente consumido
            CreateMap<DetalleConsumo, DetalleConsumoDTO>();

            //Ingredinte para listado
            CreateMap<IngredienteFaltante, IngredienteFaltanteDTO>();
        }
    }
}