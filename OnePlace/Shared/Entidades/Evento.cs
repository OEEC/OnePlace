using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnePlace.Shared.Entidades
{
    public class Evento
    {
        public int EventoId { get; set; }      
        public string NombreEvento { get; set; }
        public string TituloEvento { get; set; }
        public string DescripcionEvento { get; set; }
        public string ImgEvento { get; set; }
        public DateTime? FechaEvento { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public bool Activo { get; set; }
        public TipoEvento TipoEventos { get; set; }
    }
    public enum TipoEvento
    {
        Efemerides,
        [Description("Evento Social")]
        EventoSocial
    }
}
