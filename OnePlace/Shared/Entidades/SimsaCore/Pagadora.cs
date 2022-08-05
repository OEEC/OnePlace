using System;
using System.Collections.Generic;

namespace OnePlace.Shared.Entidades.SimsaCore
{
    public partial class Pagadora
    {
        public int Idpagadora { get; set; }
        public string Pagadora1 { get; set; }
        public int? Idestatus { get; set; }
        public DateTime? Fchcreacion { get; set; }
        public DateTime? Fchbaja { get; set; }
        public DateTime? Fchmod { get; set; }
        public int? Idusuario { get; set; }
    }
}
