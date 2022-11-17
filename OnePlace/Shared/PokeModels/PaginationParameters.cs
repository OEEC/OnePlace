using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnePlace.Shared.PokeModels
{
    public class PaginationParameters
    {
        public int PageNumber { get; set; } = 1;//numero de pagina seleccionada por default la 1
        public int PageSize { get; set; } = 6;//pokemones a mostrar por pagina

        //el offset inicial es de cero, para devolver los primeros registros de la lista
        //cuando la pagina sea 2 el offset va hacer el ultimo registro en que se quedo la pagina 1
        public int Offset => (PageNumber - 1) * PageSize;
    }
}
