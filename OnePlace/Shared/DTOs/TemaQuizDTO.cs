using OnePlace.Shared.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnePlace.Shared.DTOs
{
    //dto para poder guardar un tema y un quiz al mismo tiempo
    public class TemaQuizDTO
    {
        public Tema Tema { get; set; }
        public Quiz Quiz { get; set; }        
    }
}
