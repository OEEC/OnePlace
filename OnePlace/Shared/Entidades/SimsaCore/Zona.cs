using System;
using System.Collections.Generic;

namespace OnePlace.Shared.Entidades.SimsaCore
{
    public partial class Zona
    {
        public int Idzona { get; set; }
        public string Zona1 { get; set; }
        public int? Idestatus { get; set; }
        public DateTime? Fchcreacion { get; set; }
        public DateTime? Fchbaja { get; set; }
        public DateTime? Fchmod { get; set; }
        public int? Idusuario { get; set; }
    }
}
