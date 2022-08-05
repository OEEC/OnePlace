using System;
using System.Collections.Generic;

namespace OnePlace.Shared.Entidades.SimsaCore
{
    public partial class Marca
    {
        public int Idmarca { get; set; }
        public string Img { get; set; }
        public string Marca1 { get; set; }
        public int? Idempresa { get; set; }
        public int? Idtipo { get; set; }
        public int? Idestatus { get; set; }
        public DateTime? Fchcreacion { get; set; }
        public DateTime? Fchmod { get; set; }
        public DateTime? Fchbaja { get; set; }
        public int? Idusuario { get; set; }
    }
}
