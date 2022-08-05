using System;
using System.Collections.Generic;

namespace OnePlace.Shared.Entidades.SimsaCore
{
    public partial class Empleado
    {
        public int Idempleado { get; set; }
        public int? Idpersona { get; set; }
        public string Img { get; set; }
        public string Noemp { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
        public int? Iddepartamento { get; set; }
        public int? Idarea { get; set; }
        public int? Idpuesto { get; set; }
        public double? Nomina { get; set; }
        public double? Variable { get; set; }
        public int? Idtipo { get; set; }
        public string Idestatus { get; set; }
        public DateTime? Fchalta { get; set; }
        public DateTime? Fchbaja { get; set; }
    }
}
