using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnePlace.Shared.Entidades
{
    public class TemaFase
    {
        public int TemaId { get; set; }
        public int FaseCursoId { get; set; }
        public Tema Tema { get; set; }
        public FaseCurso FaseCurso { get; set; }

        //se le agrego el valor bool aqui ya que esta data es la que se debe de actualizar ya que aqui es la relacion tema & fase
        public bool IsComplete { get; set; }
    }
}
