using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnePlace.Client.Helpers
{
    public static class IJSRuntimeExtensionMethods
    {
        //hacer un metodo de extension para reutilizar el confirm al borrar cualquier cosa
        public static async ValueTask<bool> Confirm(this IJSRuntime JS, string titulo, string mensaje, TipoMensajeSweetAlert tipoMensajeSweetAlert)
        {
            /*aqui con await esperamos el resultado de la promesa que como valor nos devolvera un bool pasamos como parametro el nombre del
             metodo, el titulo,el mensaje y el tipo de valor que maneja sweetalert*/
            return await JS.InvokeAsync<bool>("CustomConfirm", titulo, mensaje, tipoMensajeSweetAlert.ToString());
        }
        //creamos un enum con los tipos que maneja sweetalert para pasarselos al metodo Confirm
        public enum TipoMensajeSweetAlert
        {
            question, warning, error, success, info
        }

        //para descargar archivos
        public static ValueTask<object> GuardarComo(this IJSRuntime js, string nombreArchivo, byte[] archivo)
        {
            return js.InvokeAsync<object>("saveAsFile",
                nombreArchivo,
                Convert.ToBase64String(archivo));
        }

        //metodos de extension para trabajar con localstorage
        //con set localstorage podremos guardar contenido 
        public static ValueTask<object> SetInLocalStorage(this IJSRuntime js, string key, string content)
        => js.InvokeAsync<object>("localStorage.setItem", key, content);
        //con getlocalstorage obtenemos contenido
        public static ValueTask<string> GetFromLocalStorage(this IJSRuntime js, string key)
            => js.InvokeAsync<string>("localStorage.getItem", key);
        //con remove eliminamos contenido
        public static ValueTask<object> RemoveItem(this IJSRuntime js, string key)
            => js.InvokeAsync<object>("localStorage.removeItem", key);
        public static async ValueTask<bool> PlayVideo(this IJSRuntime JS)
        {           
            return await JS.InvokeAsync<bool>("GetEventVideo");           
        }
    }
}
