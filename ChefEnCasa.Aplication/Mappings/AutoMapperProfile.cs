using AutoMapper;
using ChefEnCasa.Application.DTOs;
using ChefEnCasa.Domain.Entities;

namespace ChefEnCasa.Application.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // USUARIO
            CreateMap<Usuario, UsuarioDTO>();
            CreateMap<UsuarioCreateDTO, Usuario>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()) // El hash se maneja en el Service
                .ForMember(dest => dest.UsuarioId, opt => opt.Ignore());
        }
    }
}