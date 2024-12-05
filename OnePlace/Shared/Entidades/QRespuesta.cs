using OnePlace.Shared.Entidades.SimsaCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnePlace.Shared.Entidades
{
    public class QRespuesta
    {
        [Key]
        public int IdRespuesta { get; set; }
        public int? PreguntaId { get; set; }
        public string? UsuarioId { get; set; }
        public string? Respuesta { get; set; }
        public DateTime? fecha { get; set; }
        [NotMapped]
        public QPreguntas Pregunta { get; set;}
    }
}
