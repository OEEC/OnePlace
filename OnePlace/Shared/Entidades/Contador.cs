using Microsoft.EntityFrameworkCore;
using OnePlace.Shared.Entidades.SimsaCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnePlace.Shared.Entidades
{
    public class Contador
    {
        public long Id { get; set; }
        public int Id_Estacion { get; set; }
        public int Id_Empleado { get; set; }
        public DateTime Fecha { get; set; }
        public int Id_Accion { get; set; }

        public Estacion Estacion { get; set; }
        public Empleado Empleado { get; set; }
        public Accion Accion { get; set; }
    }
}
