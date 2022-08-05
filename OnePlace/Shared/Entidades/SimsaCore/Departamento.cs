using System;
using System.Collections.Generic;

namespace OnePlace.Shared.Entidades.SimsaCore
{
    public partial class Departamento
    {
        public int Iddepartamento { get; set; }
        public int? Idempresa { get; set; }
        public string Departamento1 { get; set; }
        public DateTime? Fchcreacion { get; set; }
        public DateTime? Fchbaja { get; set; }
        public DateTime? Fchmod { get; set; }
        public int? Idusuario { get; set; }
    }
}
