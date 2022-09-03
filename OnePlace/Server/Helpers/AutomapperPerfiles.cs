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

            CreateMap<Tema, Tema>().ForMember(x => x.Imagen, option => option.Ignore());

            /*Se ignoro el tema por que quiz tiene ligado un objeto tema el cual en la api de update, primero se actualiza el tema 
            luego el quiz y como quiz tiene tema me pone nuevamente los valores del tema que el trae y no de que se actualizo previamente*/
            CreateMap<Quiz, Quiz>()
                .ForMember(x => x.Imagen, option => option.Ignore())
                .ForMember(x => x.Tema, option => option.Ignore());

        }
    }
}
