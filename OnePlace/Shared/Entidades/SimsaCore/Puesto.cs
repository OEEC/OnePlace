using System;
using System.Collections.Generic;

namespace OnePlace.Shared.Entidades.SimsaCore
{
    public partial class Puesto
    {
        public int Idpuesto { get; set; }
        public string Puesto1 { get; set; }   
        public DateTime? Fchcreacion { get; set; }
        public DateTime? Fchbaja { get; set; }
        public DateTime? Fchmod { get; set; }
        public int? Idusuario { get; set; }
    }
}
