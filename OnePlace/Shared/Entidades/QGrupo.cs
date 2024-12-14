using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnePlace.Shared.Entidades
{
    public class QGrupo
    {
        [Key]
        public int Idgrupo { get; set; }
        public string? Grupo { get; set; }

        // Propiedad de navegación: Un grupo puede tener muchas preguntas
        public List<QPreguntas>? Preguntas { get; set; }

    }
}
