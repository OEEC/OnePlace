using OnePlace.Shared.Entidades.SimsaCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnePlace.Shared.Entidades
{
    public class PromocionZona
    {
        public int PromocionId { get; set; }
        public int ZonaId { get; set; }
        public Promocion Promocion { get; set; }
        public Zona Zona { get; set; }
    }
}
