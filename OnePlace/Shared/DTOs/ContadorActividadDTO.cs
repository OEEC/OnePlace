using OnePlace.Shared.Filtros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnePlace.Shared.DTOs
{
    public class ContadorActividadDTO : Parametros_Busqueda_Gen
    {
        public string Estacion { get; set; }
        public string Empleado { get; set; }
        public string Persona { get; set; }
        public string Accion { get; set; }
        public DateTime Fecha_Registro { get; set; }
    }
}
