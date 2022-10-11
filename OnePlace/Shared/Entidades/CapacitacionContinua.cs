using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnePlace.Shared.Entidades
{
    public class CapacitacionContinua
    {
        public int CapacitacionContinuaId { get; set; }
        public string Nombre { get; set; }      
        public string Descripcion { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public string Imagen { get; set; }
        public bool Activo { get; set; }
        public List<VideosCapacitacion> ListadeVideos { get; set; }
        public List<CapacitacionContinuaZona> CapacitacionContinuaZona { get; set; } = new List<CapacitacionContinuaZona>();
    }
    public class VideosCapacitacion
    {
        [Key]
        public int VideosCapacitacionId { get; set; }
        public int CapacitacionContinuaId { get; set; }
        public int ArchivoId { get; set; }
    }
}
