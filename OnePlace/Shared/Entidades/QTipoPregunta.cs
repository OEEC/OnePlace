using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnePlace.Shared.Entidades
{
    public class QTipoPregunta
    {
        [Key]
        public int IdTipoPregunta { get; set; }
        public string Tipo { get; set; }
        [NotMapped]
        public QTipoRespuesta? TipoRespuesta { get; set; }
    }
}
