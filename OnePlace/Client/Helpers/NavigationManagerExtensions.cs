using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnePlace.Client.Helpers
{
    public static class NavigationManagerExtensions
    {
        //retornamos un diccionario
        public static Dictionary<string, string> ObtenerQueryStrings(this NavigationManager navigationManager, string url)
        {
            if (string.IsNullOrWhiteSpace(url) || !url.Contains("?") || url.Substring(url.Length - 1) == "?")
            {
                return null;
            }
            //esto lo que hace es que construye los querystring apartir de un arreglo de valores
            //usamos split para separar la url ejemplo: https://dominio.com?llave1=valor1&llave2=valor2
            var queryStrings = url.Split(new string[] { "?" }, StringSplitOptions.None)[1];
            Dictionary<string, string> dicQueryString =
                                                    queryStrings.Split('&')
                                                         .ToDictionary(c => c.Split('=')[0],
                                                                       c => Uri.UnescapeDataString(c.Split('=')[1]));

            return dicQueryString;//aqui ya tenemos construido el diccionariostring
        }
    }
}
