using OnePlace.Shared.Entidades.SimsaCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnePlace.Shared.Entidades
{
    //relacion uno a muchos
    public class Promocion
    {
        public int PromocionId { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public DateTime? FechadeRegistro { get; set; }
        public DateTime? FechadeTermino { get; set; }
        public TipodePromocion TipodePromociones { get; set; }
        public LugardeVisualizacion LugardeVisualizacion { get; set; }
        public bool Activo { get; set; }
        public bool Mostrar { get; set; }

        //propiedades de navegacion
        public List<ImagenesCarrusel> Imagenes { get; set; }
        public int ZonaId { get; set; }

        //no hay fk con zonas por que zonas es MYISAM y Promocion es INNODB
        //[NotMapped]
        public virtual Zona Zona { get; set; }
    }
    public class ImagenesCarrusel
    {
        public int ImagenesCarruselId { get; set; }
        public string Imagen { get; set; }
        public int PromocionId { get; set; }//propiedad de navegacion   
    }
    public enum LugardeVisualizacion
    {
        [Description("Pantalla Principal")]
        PantallaPrincipal,
    }
    public enum TipodePromocion
    {
        [Description("Promoción")]
        Promocion,
        [Description("Convenio")]
        Convenio,
        [Description("Oferta")]
        Oferta
    }
}
