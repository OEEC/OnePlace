using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnePlace.Shared.DTOs
{
    public class PaginadorGenerico<T> where T : class
    {
        //Página devuelta por la consulta actual.       
        public int PaginaActual { get; set; }

        //Número de registros de la página devuelta.     
        public int RegistrosPorPagina { get; set; }

        //Total de registros de consulta.       
        public int TotalRegistros { get; set; }

        //Total de páginas de la consulta.    
        public int TotalPaginas { get; set; }

        //Texto de búsqueda de la consuta actual.  
        public string BusquedaActual { get; set; }

        //Columna por la que esta ordenada la consulta actual.      
        public string OrdenActual { get; set; }

        //Tipo de ordenación de la consulta actual: ASC o DESC.      
        public string TipoOrdenActual { get; set; }       

        //Resultado devuelto por la consulta a la coleccion Xmls
        //en función de todos los parámetros anteriores.      
        public IEnumerable<T> Resultado { get; set; }

        //resultado de un ienumerable que contiene todos los registros sin parametros de paginacion
        public IEnumerable<T> ResultadoAExportar { get; set; }
    }
}
