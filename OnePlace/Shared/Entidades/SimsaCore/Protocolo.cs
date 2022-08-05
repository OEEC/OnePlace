using System;
using System.Collections.Generic;

namespace OnePlace.Shared.Entidades.SimsaCore
{
    public partial class Protocolo
    {
        public int Idprotocolo { get; set; }
        public int? Idempleado { get; set; }
        public string Empleado { get; set; }
        public int? Idestacion { get; set; }
        public double? Decalogo { get; set; }
        public double? Imagen { get; set; }
        public double? Limpieza { get; set; }
        public double? Promociones { get; set; }
        public double? Puntualidad { get; set; }
        public double? Total { get; set; }
        public double? Importe { get; set; }
        public DateTime? Fchcal { get; set; }
    }
}
