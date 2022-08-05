using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnePlace.Client.Helpers
{
    public class MostrarMensajes : IMostrarMensaje
    {
        //inyectamos jsruntime
        private readonly IJSRuntime js;
        public MostrarMensajes(IJSRuntime js)
        {
            this.js = js;//lo inicializamos como un campo
        }
        public async Task MostrarMensajeAdvertencia(string mensaje)
        {
            await MostrarMensaje("Aviso", mensaje, "warning");
        }
        //esta clase implementa a la interfar IMostrarMensjaes
        public async Task MostrarMensajeError(string mensaje)
        {
            await MostrarMensaje("Error", mensaje, "error");
        }
        public async Task MostrarMensajeExitoso(string mensaje)
        {
            await MostrarMensaje("Éxito", mensaje, "success");
        }

        //creamos un metodo que utilizaran los dos metodos de arriba aqui va el sweetalert
        private async ValueTask MostrarMensaje(string titulo, string mensaje, string tipoMensaje)
        {
            await js.InvokeVoidAsync("Swal.fire", titulo, mensaje, tipoMensaje);
        }
    }
}
