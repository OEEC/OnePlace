using OnePlace.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnePlace.Server.Helpers
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> Paginar<T>(this IQueryable<T> queryable, PaginacionDTO paginacion)
        {
            /*usamos skip que significa saltar y nos saltaremos una cantidad de registros que va a ser la
            multiplicacion de paginacion -1 por la cantidad total de registros*/
            return queryable
                .Skip((paginacion.Pagina - 1) * paginacion.CantidadRegistros)
                .Take(paginacion.CantidadRegistros);
        }
    }
}
