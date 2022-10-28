using OnePlace.Shared.Entidades.SimsaCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Descripcion { get; set; }
        public DateTime? FechadeRegistro { get; set; }

        [Required(ErrorMessage = "El campo Fecha Termino es requerido")]
        public DateTime? FechadeTermino { get; set; }
        public TipodePromocion TipodePromociones { get; set; }
        public LugardeVisualizacion LugardeVisualizacion { get; set; }
        public bool Activo { get; set; }
        public bool Mostrar { get; set; }

        //propiedades de navegacion
        public List<ImagenesCarrusel> Imagenes { get; set; }
        public List<PromocionZona> PromocionZona { get; set; } = new List<PromocionZona>();        
    }
    public class ImagenesCarrusel
    {
        public int ImagenesCarruselId { get; set; }
        public string Imagen { get; set; }
        //propiedad de navegacion       
        public int PromocionId { get; set; }   
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
