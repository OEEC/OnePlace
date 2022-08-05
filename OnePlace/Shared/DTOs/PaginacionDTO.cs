using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnePlace.Shared.DTOs
{
    public class PaginacionDTO
    {
        //la paginacion se crea para no mostrar todos los registros de la bd, sino mejor por pagina con una cierta cantidad de datos a la vista

        public int Pagina { get; set; } = 1;//la pagina inicial es la 1
        public int CantidadRegistros { get; set; } = 10;//la cantidad de registros a mostrar es 10
    }
}
