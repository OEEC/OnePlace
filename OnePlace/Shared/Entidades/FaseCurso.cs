using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnePlace.Shared.Entidades
{
    public class FaseCurso
    {
        public int FaseCursoId { get; set; }
        public string FaseNombre { get; set; }
        public string Icono { get; set; }    

        //propiedades de navegacion
        public List<TemaFase> TemaFase { get; set; }
    }
}
