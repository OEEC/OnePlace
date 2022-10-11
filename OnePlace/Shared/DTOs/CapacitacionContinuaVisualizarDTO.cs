using OnePlace.Shared.Entidades;
using OnePlace.Shared.Entidades.SimsaCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnePlace.Shared.DTOs
{
    public class CapacitacionContinuaVisualizarDTO
    {
        public CapacitacionContinua CapacitacionContinua { get; set; }
        public List<Zona> Zonas { get; set; }
    }
}
