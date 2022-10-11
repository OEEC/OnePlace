using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnePlace.Shared.Entidades.SimsaCore
{
    public class Zona
    {       
        public int ZonaId { get; set; }
        public string Zona1 { get; set; }
        public int? Idestatus { get; set; }
        public DateTime? Fchcreacion { get; set; }
        public DateTime? Fchbaja { get; set; }
        public DateTime? Fchmod { get; set; }
        public int? Idusuario { get; set; }
        public List<PromocionZona> PromocionZona { get; set; }
        public List<CapacitacionContinuaZona> CapacitacionContinuaZona { get; set; }
    }
}
