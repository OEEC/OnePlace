using OnePlace.Shared.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnePlace.Shared.DTOs
{
    //este dto se creo con el unico fin de pasar una bandera de termino de fases
    public class ActividadesFasesTerminadasDTO
    {
        public List<ActividadUsuario> ListadeActiviades {get; set;}
        public bool Terminado { get; set; }
    }
}
