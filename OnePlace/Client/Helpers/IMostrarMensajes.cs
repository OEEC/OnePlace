using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnePlace.Client.Helpers
{
    public interface IMostrarMensaje
    {
        //esto es para tener un modelo de mensaje de error generico y ponerlos en todas las respuestas de error de cada una de las peticiones por post
        Task MostrarMensajeError(string mensaje);
        Task MostrarMensajeExitoso(string mensaje);
        Task MostrarMensajeAdvertencia(string mensaje);
    }
}
