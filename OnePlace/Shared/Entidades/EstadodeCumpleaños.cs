using OnePlace.Shared.Entidades.SimsaCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnePlace.Shared.Entidades
{
    public class EstadodeCumpleaños
    {
        public int EstadodeCumpleañosId { get; set; }
        public bool TarjetaDescargada { get; set; }
        public string UserId { get; set; }       
        public int Idempleado { get; set; }

        //el not mapped hace que la bd no quiera poner foreign key con la tabla empleados , no se puede por que una tabla es innodb y la otra myasam
        [NotMapped]
        public Empleado Empleado { get; set; }
    }
}
