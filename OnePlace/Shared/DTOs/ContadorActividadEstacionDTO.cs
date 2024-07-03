using OnePlace.Shared.Filtros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnePlace.Shared.DTOs
{
    public class ContadorActividadEstacionDTO : Parametros_Busqueda_Gen
    {
        public string Estacion { get; set; }
        public int Cantidad { get; set; }
        public DateTime Fecha { get; set; }
    }
}
