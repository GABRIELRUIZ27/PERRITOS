using AutoMapper;
using Perritos.DTOs;
using Perritos.Entities;
using static System.Collections.Specialized.BitVector32;
using System.ComponentModel;

namespace Perritos.Utilities 
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            // source, destination
            CreateMap<UsuarioDTO, Usuario>();
            CreateMap<Usuario, UsuarioDTO>()
                .ForMember(dest => dest.Rol, opt => opt.MapFrom(src => src.Rol));

            CreateMap<Rol, RolDTO>();
            CreateMap<RolDTO, Rol>();

            CreateMap<Genero, GeneroDTO>();
            CreateMap<GeneroDTO, Genero>();

            CreateMap<Discapacidad, DiscapacidadDTO>();
            CreateMap<DiscapacidadDTO, Discapacidad>();

            CreateMap<Adoptado, AdoptadoDTO>()
                .ForMember(dest => dest.Perrito, opt => opt.MapFrom(src => src.Perrito));
            CreateMap<AdoptadoDTO, Adoptado>();

            CreateMap<Claim, ClaimDTO>()
                .ForMember(dest => dest.RolId, opt => opt.MapFrom(src => src.Rol.Id))
                .IncludeMembers(src => src.Rol);

            CreateMap<Rol, ClaimDTO>()
                .ForMember(dest => dest.RolId, opt => opt.MapFrom(src => src.Id));

            CreateMap<PerritoDTO, Perrito>();
            CreateMap<Perrito, PerritoDTO>()
                .ForMember(dest => dest.Genero, opt => opt.MapFrom(src => src.Genero))
                .ForMember(dest => dest.Discapacidad, opt => opt.MapFrom(src => src.Discapacidad));
        }
    }
}