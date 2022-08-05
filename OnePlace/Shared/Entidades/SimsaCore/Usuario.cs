using System;
using System.Collections.Generic;

namespace OnePlace.Shared.Entidades.SimsaCore
{
    public partial class Usuario
    {
        public int Idusuario { get; set; }
        public int? Idempleado { get; set; }
        public string Usuario1 { get; set; }
        public string Contrasena { get; set; }
        public int? Idtipo { get; set; }
        public int? FkId { get; set; }
    }
}
