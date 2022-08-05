using System;
using System.Collections.Generic;

namespace OnePlace.Shared.Entidades.SimsaCore
{
    public partial class Area
    {
        public int Idarea { get; set; }
        public int? Iddepartamento { get; set; }
        public string Area1 { get; set; }
        public DateTime? Fchcreacion { get; set; }
        public DateTime? Fchmod { get; set; }
        public DateTime? Fchbaja { get; set; }
        public int? Idusuario { get; set; }
    }
}
