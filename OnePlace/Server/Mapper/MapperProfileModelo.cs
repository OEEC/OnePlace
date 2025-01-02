using AutoMapper;
using OnePlace.Shared.DTOs;
using OnePlace.Shared.DTOs.Modelos;
using OnePlace.Shared.Entidades;
using OnePlace.Shared.Entidades.SimsaCore;
using OnePlace.Shared.IdentityModels;

namespace OnePlace.Server.Mapper
{
    public class MapperProfileModelo : Profile
    {
        public MapperProfileModelo()
        {
            CreateMap<Empleado, EmpleadoDTO>();
            CreateMap<IdentityUsuario, UsuarioAspDTO>();
            CreateMap<QGrupo, QzGrupoDTO>();
            CreateMap<QPreguntas, QzPreguntaDTO>();
            CreateMap<QRespuesta, QzRespuestaDTO>();
            CreateMap<QRespuesta, QzRespuestaSimpleDTO>();
            CreateMap<QTipoPregunta, QzTipoPreguntaDTO>();
            CreateMap<QTipoRespuesta, QzTipoRespuestaDTO>();
            CreateMap<Zona, ZonaDTO>()
                .ForMember(x => x.Id, opt => opt.MapFrom(y => y.ZonaId))
                .ForMember(x => x.Zona, opt => opt.MapFrom(y => y.Zona1));
            CreateMap<Estacion, EstacionDTO>()
                .ForMember(x => x.Id, opt => opt.MapFrom(y => y.Idestacion));
            CreateMap<Departamento, DepartamentoDTO>()
                .ForMember(x => x.Id, opt => opt.MapFrom(y => y.Iddepartamento))
                .ForMember(x => x.Departamento, opt => opt.MapFrom(y => y.Departamento1));
            CreateMap<Puesto, PuestoDTO>()
                .ForMember(x => x.Id, opt => opt.MapFrom(y => y.Idpuesto))
                .ForMember(x => x.Puesto, opt => opt.MapFrom(y => y.Puesto1));
        }
    }
}
