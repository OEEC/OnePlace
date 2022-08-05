using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OnePlace.Server.Helpers
{
    public class AlmacenadorArchivosLocal : IAlmacenadorArchivos
    {
        private readonly IWebHostEnvironment env;//poder sacar la url de donde se encuentra la carpeta que almacena la imagen
        private readonly IHttpContextAccessor httpContextAccessor;

        public AlmacenadorArchivosLocal(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            this.env = env;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> EditarArchivo(byte[] contenido, string extension, string nombreContenedor, string rutaArchivoActual)
        {
            if (!string.IsNullOrEmpty(rutaArchivoActual))
            {
                await EliminarArchivo(rutaArchivoActual, nombreContenedor);
            }

            return await GuardarArchivo(contenido, extension, nombreContenedor);
        }

        public Task EliminarArchivo(string ruta, string nombreContenedor)
        {
            var filename = Path.GetFileName(ruta);//obtenemos el nombre de la ruta que viene de la base de datos
            string directorioArchivo = Path.Combine(env.WebRootPath, nombreContenedor, filename);
            if (File.Exists(directorioArchivo))
            {
                //si el archivo exite borramos el archivo
                File.Delete(directorioArchivo);
            }

            return Task.FromResult(0);
        }

        public async Task<string> GuardarArchivo(byte[] contenido, string extension, string nombreContenedor)
        {
            var filename = $"{Guid.NewGuid()}.{extension}";//nombre del archivo
            string folder = Path.Combine(env.WebRootPath, nombreContenedor);//carpeta donde se va a colocar la imagen

            //sino exite el directorio tenemos que crearlo
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string rutaGuardado = Path.Combine(folder, filename);//combinamos el nombre de la carpeta con el del archivo
            await File.WriteAllBytesAsync(rutaGuardado, contenido);//escribimo los bytes en el sistema

            //url de la imagen que el navegador puede leer
            var urlActual = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}";
            var rutaParaBD = Path.Combine(urlActual, nombreContenedor, filename);
            return rutaParaBD;
        }
    }
}