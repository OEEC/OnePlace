using OnePlace.Shared.Entidades.SimsaCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnePlace.Shared.Entidades
{
    public class CursoZona
    {
        public int Id_Zona { get; set; }
        public int Id_Curso { get; set; }

        [NotMapped] public Zona Zona { get; set; }
        [NotMapped] public Curso Curso { get; set; }
    }
}
