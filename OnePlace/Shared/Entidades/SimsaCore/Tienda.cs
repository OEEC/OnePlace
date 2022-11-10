using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnePlace.Shared.Entidades.SimsaCore
{
    public class Tienda
    {
        public int TiendaId { get; set; }
        public string Nombre { get; set; }

        //propiedades de navegacion
        public List<Empleado> Empleados { get; set; }
    }
}
