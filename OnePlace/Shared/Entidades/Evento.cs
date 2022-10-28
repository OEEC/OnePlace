using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnePlace.Shared.Entidades
{
    public class Evento
    {
        public int EventoId { get; set; }

        [Required(ErrorMessage = "El campo Nombre Evento es requerido")]
        public string NombreEvento { get; set; }
        public string TituloEvento { get; set; }

        [Required(ErrorMessage = "El campo Descripción Evento es requerido")]
        public string DescripcionEvento { get; set; }
        public string ImgEvento { get; set; }

        [Required(ErrorMessage = "El campo Fecha Evento es requerido")]
        public DateTime? FechaEvento { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public bool Activo { get; set; }
        public TipoEvento TipoEventos { get; set; }
        public string TituloEventoExtra { get; set; }
        public string DescripcionEventoExtra { get; set; }

        //propiedades de navegacion
        public List<ImagenesCarruselEvento> Imagenes { get; set; }
    }
    public enum TipoEvento
    {
        Efemerides,
        [Description("Evento Social")]
        EventoSocial
    }
    public class ImagenesCarruselEvento
    {
        public int ImagenesCarruselEventoId { get; set; }
        public string Imagen { get; set; }
        public int EventoId { get; set; }
    }
}
