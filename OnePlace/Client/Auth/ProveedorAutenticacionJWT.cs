using OnePlace.Client.Helpers;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using OnePlace.Client.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace OnePlace.Client.Auth
{
    public class ProveedorAutenticacionJWT : AuthenticationStateProvider, ILoginService
    {   //inyectamos jsruntime
        public ProveedorAutenticacionJWT(IJSRuntime js, HttpClient httpClient)
        {
            //lo inicializamos como un campo
            this.js = js;
            this.httpClient = httpClient;
        }

        public static readonly string ShowModal = "ShowModal";//nombre de la key que se necesita para guardar datos en localstorage
        public string FlagShowModal = "true";// value que se necesita para guardar en localstorage, en este caso se guarda un string ya que no se pueden guardar bool

        //le pasamos una llave a localstorage
        public static readonly string TOKENKEY = "TOKENKEY";       
        private readonly IJSRuntime js;
        private readonly HttpClient httpClient;

        //creamos el usuario anonimo
        private AuthenticationState Anonimo => new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

        //este metodo me va a permitr saber si estoy autenticado mediante un token que va a recibir
        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            //verificamos si ya hay un token en localstorage
            var token = await js.GetFromLocalStorage(TOKENKEY);

            //si el usuario no tiene un token es un usuario anonimo
            if (string.IsNullOrEmpty(token))
            {
                return Anonimo;
            }
            return ConstruirAuthenticationState(token);
        }
        //si tiene un token lo vamos a utilizar para crear el estado de autenticacion, creamos un metodo que recibe como parametro el token
        private AuthenticationState ConstruirAuthenticationState(string token)
        {
            //colocamos en la cabecera que recibimos de http el token de localstorage asi podemos autenticarnos en cada peticion http que hagamos
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            //retornamos el token del cual vamos a extraer los claims usamos el metodo parseclaimsfromjwt de microsoft
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt")));
        }

        //este metodo lo creo el equipo de microsoft para trabajar con los claims aun no esta incluido en blazor 
        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            keyValuePairs.TryGetValue(ClaimTypes.Role, out object roles);

            if (roles != null)
            {
                if (roles.ToString().Trim().StartsWith("["))
                {
                    var parsedRoles = JsonSerializer.Deserialize<string[]>(roles.ToString());

                    foreach (var parsedRole in parsedRoles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, parsedRole));
                    }
                }
                else
                {
                    claims.Add(new Claim(ClaimTypes.Role, roles.ToString()));
                }

                keyValuePairs.Remove(ClaimTypes.Role);
            }

            claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString())));
            return claims;
        }

        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }

        //aqui implementamos la interfaz de loginservice
        public async Task Login(string token)
        {
            //guardar un valor en localstorage (para que al loguearse me guarde un string que funcionara como bool en el modal de cumpleaños)
            await js.SetInLocalStorage(ShowModal, FlagShowModal);

            await js.SetInLocalStorage(TOKENKEY, token);//guardamos el token en localstorage
            var authState = ConstruirAuthenticationState(token);//construimos el estado de autenticacion
            //con esto notificamos a blazor que el estado de autenticacion del usuario a cambiado
            NotifyAuthenticationStateChanged(Task.FromResult(authState));
        }

        public async Task Logout()
        {
            await js.RemoveItem(ShowModal);//eliminamos la variable showmodal para cuando el usuario administrador se desloguie ya que la variable se quedaria por no tener un boton close modal
            await js.RemoveItem(TOKENKEY);//eliminamos el token de localstorage
            httpClient.DefaultRequestHeaders.Authorization = null;//quitamos de la cabecera http el token
            NotifyAuthenticationStateChanged(Task.FromResult(Anonimo));//notificamos a blazor que hubo un cambio en el estado
        }
    }
}
