using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnePlace.Shared.Entidades
{
    //clase para que un tema tenga varias fases
    public class TemaFase
    {
        public int TemaId { get; set; }
        public int FaseCursoId { get; set; }
        public Tema Tema { get; set; }
        public FaseCurso FaseCurso { get; set; }        
    }
}
