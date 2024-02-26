using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OnePlace.Client.Auth;
using OnePlace.Client.ComponentesGenericos.Services;
using OnePlace.Client.Helpers;
using OnePlace.Client.Repositorios;
using OnePlace.Client.Service;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OnePlace.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            //builder.Services.AddBaseAddressHttpClient();
            builder.Services.AddSingleton(new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress), Timeout = TimeSpan.FromMinutes(15) });

            //llamar al metodo ConfigureServices
            ConfigureServices(builder.Services);
            await builder.Build().RunAsync();
        }

        //configurar el sistema de inyeccion de dependencias del lado del cliente blazor
        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();//configurar el sistema de autorizacion
            services.AddSingleton<ServiciosSingleton>();
            services.AddTransient<ServicioTransient>();

            //poner la interface y el servicio de repositorios
            services.AddScoped<IRepositorio, Repositorio>();

            //servicio para consumir api rest externa del lado del cliente
            services.AddHttpClient<ISimsacoreService, SimsaCoreService>(client =>
            {
                client.BaseAddress = new Uri("http://localhost/api_limpieza");
            });

            //ponemos la instancia a servir cuando se nos pida un IMostrarMensajes 
            services.AddScoped<IMostrarMensaje, MostrarMensajes>();

            //servicio para componentes 
            services.AddScoped<IJsApiService, JsApiService>();

            //servicio para boton de quitar navmenu
            services.AddSingleton<ViewOptionService>();

            //agregar las funciondes de Microsoft.AspNetCore.Components.Authorization
            services.AddAuthorizationCore();

            //creamos una instancia en nuestro sistema de dependencias del proveedor de autenticacion
            services.AddScoped<ProveedorAutenticacionJWT>();

            //agregamos un servicio de autenticacion del estado, y le pasamos la instancia de arriba
            services.AddScoped<AuthenticationStateProvider, ProveedorAutenticacionJWT>(
                provider => provider.GetRequiredService<ProveedorAutenticacionJWT>());

            //ponemos el servicio de loginservice y le pasamos la instancia de proveedorautenticacionjwt
            services.AddScoped<ILoginService, ProveedorAutenticacionJWT>(
               provider => provider.GetRequiredService<ProveedorAutenticacionJWT>());
        }
    }
}
