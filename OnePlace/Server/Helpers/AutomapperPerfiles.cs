using AutoMapper;
using OnePlace.Shared.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnePlace.Server.Helpers
{
    public class AutomapperPerfiles : Profile
    {
        //atravez de los perfiles es que automaper puede definir las reglas de mapeo,creamos un constructor      
        public AutomapperPerfiles()
        {
            CreateMap<Promocion, Promocion>().ForMember(x => x.Imagenes, option => option.Ignore());

            CreateMap<Evento, Evento>()
                .ForMember(x => x.Imagenes, option => option.Ignore())
                .ForMember(x => x.ImgEvento, option => option.Ignore());
        }
    }
}
