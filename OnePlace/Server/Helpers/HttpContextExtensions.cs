using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnePlace.Server.Helpers
{
    public static class HttpContextExtensions
    {
        //este metodo es reusable para todos los componentes
        //el maximo de registros existentes lo vamos a colocar en la cabecera de la respuesta http
        public async static Task InsertarParametrosPaginacionEnRespuesta<T>(this HttpContext context,
        IQueryable<T> queryable, int cantidadRegistrosAMostrar, bool sincontext = false)//el bool sincontext se inicializa en false para hacerlo opcional
        {
            if (context == null) { throw new ArgumentNullException(nameof(context)); }

            double conteo;
            if (sincontext == true)
            {
                //ERROR:
                //The provider for the source IQueryable doesn't implement IAsyncQueryProvider. 
                //Only providers that implement IAsyncQueryProvider can be used for Entity Framework asynchronous operations.
                conteo = queryable.Count();//cuando es un new list y viene de memoria
            }
            else
            {
                //con esto contamos todos los registros
                conteo = await queryable.CountAsync();//cuando es un list de un context y viene de la bd
            }

            //aqui hacemos una division entre la cantidad total de registros de la bd y la cantidad de registros a mostrar
            double totalPaginas = Math.Ceiling(conteo / cantidadRegistrosAMostrar);
            //colocamos el total en la cabecera de la respuesta http
            context.Response.Headers.Add("conteo", conteo.ToString());
            context.Response.Headers.Add("totalPaginas", totalPaginas.ToString());
        }
    }
}
