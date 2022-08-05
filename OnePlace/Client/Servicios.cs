using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnePlace.Client
{
    //esta clase persiste la data en localstorage o en la bd
    public class ServiciosSingleton
    {
        public int Valor { get; set; }
    }

    //esta clase no persiste la data, crea una nueva al salir de la pagina donde se esta ejecutando
    public class ServicioTransient
    {
        public int Valor { get; set; }
    }
}
