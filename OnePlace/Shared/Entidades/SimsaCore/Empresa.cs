using System;
using System.Collections.Generic;

namespace OnePlace.Shared.Entidades.SimsaCore
{
    public partial class Empresa
    {
        public int Idempresa { get; set; }
        public string Rfc { get; set; }
        public string Patronal { get; set; }
        public string Razonsocial { get; set; }
        public string Domicilio { get; set; }
        public int? Idestatus { get; set; }
        public DateTime? Fchcreacion { get; set; }
        public DateTime? Fchmod { get; set; }
        public DateTime? Fchbaja { get; set; }
        public int? Idusuario { get; set; }
    }
}
