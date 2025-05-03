using AutoMapper;
using Proyecto_POO.DTOs;
using Proyecto_POO.Models;

namespace Proyecto_POO.Mappings;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<UsuarioRegisterDTO, Person>();
        CreateMap<Person, PersonDTO>().ReverseMap();
        CreateMap<User, UserDTO>();
        CreateMap<Ubicacion, UbicacionDTO>();
        CreateMap<Ubicacion, UbicacionActualDTO>();
        CreateMap<UbicacionesRequesDTO, Ubicacion>()
            .ForMember(dest => dest.Idpersona, opt => opt.MapFrom(src => src.PersonaId))
            .ForMember(dest => dest.Direccion, opt => opt.MapFrom(src => src.Direccion))
            .ForMember(dest => dest.Latitud, opt => opt.Ignore())
            .ForMember(dest => dest.Longitud, opt => opt.Ignore())
            .ForMember(dest => dest.Fecha, opt => opt.Ignore());
    }
}
