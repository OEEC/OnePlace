using OnePlace.Shared.Entidades.SimsaCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnePlace.Shared.Entidades
{
    public class CapacitacionContinuaZona
    {
        public int CapacitacionContinuaId { get; set; }
        public int ZonaId { get; set; }
        public CapacitacionContinua CapacitacionContinua { get; set; }
        public Zona Zona { get; set; }
    }
}
