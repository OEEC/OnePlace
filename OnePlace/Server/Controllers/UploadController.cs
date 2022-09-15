using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnePlace.Server.Data;
using OnePlace.Shared.Entidades;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OnePlace.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        //Necesitamos este objeto para decidir la ruta del lado del servidor para guardar los archivos cargados
        private readonly IWebHostEnvironment environment;
        private readonly IHttpContextAccessor httpContextAccessor;

        private readonly oneplaceContext context;
        private readonly UserManager<ApplicationUser> _userManager;
        public UploadController(IWebHostEnvironment environment, oneplaceContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            _userManager = userManager;
            this.environment = environment;
            this.httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrador")]
        public async Task<ActionResult<int>> Post(ArchivoAdjunto archivo)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            archivo.FechaDeSubida = DateTime.Now;

            //Obtener la extension del archivo - tipo de documento
            string extencion = archivo.NombreArchivo.ToString().Split('.').Last();
            archivo.ExtensionArchivo = extencion;

            if (extencion == "pdf")
            {
                context.Add(archivo);
                await context.SaveChangesAsync(user.Id);
            }
            else
            {
                string mensajeError = "Archivo no válido!";
                return BadRequest(mensajeError);
            }

            return archivo.ArchivoAdjuntoId;
        }

        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrador,Usuario")]
        public async Task<ActionResult<ArchivoAdjunto>> Get(int Id)
        {
            var archivo = context.ArchivoAdjuntos.Where(x => x.ArchivoAdjuntoId == Id).FirstOrDefault();
            if (archivo == null) { return NotFound(); }
            return archivo;
        }

        //creamos una peticion delete para borrar los registros
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrador")]
        public async Task<ActionResult> Delete(int id)
        {
            //verificamos si existe un cliente con ese id
            var existe = await context.ArchivoAdjuntos.AnyAsync(x => x.ArchivoAdjuntoId == id);
            if (!existe) { return NotFound(); }//sino existe retornamos notfound
            context.Remove(new ArchivoAdjunto { ArchivoAdjuntoId = id });//si exite removemos ese id de registro
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("Mostrarpdf/{Id}")]
        [AllowAnonymous]
        public IActionResult GetPdf(int Id)
        {
            //buscamos en la tabla archivo el archivo con el id envidado
            var archivo = context.ArchivoAdjuntos.Where(x => x.ArchivoAdjuntoId == Id).FirstOrDefault();

            var bytes16 = new byte[16];//inicilizamos un array de 16 bytes          
            byte[] salida = new byte[0];//y un array vacio de bytes

            if (archivo == null)
            {
                //si no se econtro el archivo retornamos notfound
                return NotFound();
            }
            else
            {
                salida = archivo.ArchivoEnBytes;// llenamos el arreglo vacio de bytes con los bytes del archivo obtenido
                return File(salida, "application/pdf");// retornamos los bytes transformados en lectura a pdf
            }
        }

        ///////////////////////////
        // SUBIR VIDEOS ///////////
        /////////////////////////// 

        [Route("Video")]
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrador")]
        public async Task<ActionResult<int>> PostVideo(ArchivoAdjunto uploadedFile)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            uploadedFile.FechaDeSubida = DateTime.Now;

            //Obtener la extension del archivo - tipo de documento
            string extencion = uploadedFile.NombreArchivo.ToString().Split('.').Last();
            uploadedFile.ExtensionArchivo = extencion;

            if (extencion == "mp4")
            {
                string nombreContenedor = "VideoServer";
                var path = $"{environment.WebRootPath}\\{nombreContenedor}\\{uploadedFile.NombreArchivo}";

                var fs = System.IO.File.Create(path);
                fs.Write(uploadedFile.ArchivoEnBytes, 0, uploadedFile.ArchivoEnBytes.Length);
                fs.Close();

                //pathbase es para obtener la url base en este caso capacitate solo cuando esta en IIS en local biene vacio no afecta
                var urlActual = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}{httpContextAccessor.HttpContext.Request.PathBase}";
                var rutaParaBD = Path.Combine(urlActual, nombreContenedor, uploadedFile.NombreArchivo);

                uploadedFile.UrlLocal = rutaParaBD;
                uploadedFile.ArchivoEnBytes = null;//vienen bytes pero se hacen null ya que causa error si se almacenan 

                context.Add(uploadedFile);
                await context.SaveChangesAsync(user.Id);
            }
            else
            {
                string mensajeError = "Archivo no válido!";
                return BadRequest(mensajeError);
            }

            return uploadedFile.ArchivoAdjuntoId;
        }

        #region VERELVIDEOPOMEDIODEBYTES
        //SI SE QUIERE VER EL VIDEO POR MEDIO DE BYTES DESCOMENTAR

        //[HttpGet("MostrarVideo/{Id}")]
        //[AllowAnonymous]
        //public IActionResult GetVi(int Id)
        //{
        //    //buscamos en la tabla archivo el archivo con el id envidado
        //    var archivo = context.ArchivoAdjuntos.Where(x => x.ArchivoAdjuntoId == Id).FirstOrDefault();

        //    var bytes16 = new byte[16];//inicilizamos un array de 16 bytes          
        //    byte[] salida = new byte[0];//y un array vacio de bytes

        //    if (archivo == null)
        //    {
        //        //si no se econtro el archivo retornamos notfound
        //        return NotFound();
        //    }
        //    else
        //    {
        //        salida = archivo.ArchivoEnBytes;// llenamos el arreglo vacio de bytes con los bytes del archivo obtenido
        //        return File(salida, "application/mp4");// retornamos los bytes transformados en lectura a mp4
        //    }
        //}

        #endregion

        //////////////////////////////////////////////////
        // APIS PARA SUBIR MULTIPLES IMAGENES/////////////
        /////////////////////////////////////////////////    

        [Route("Img")]
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrador,Usuario")]
        public async Task<ActionResult<List<int>>> PostImg(List<ArchivoAdjunto> listadearchivos)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            //lista a devolver con los id de los archivosadjuntos
            List<int> listaARetornar = new List<int>();

            //recorremos la lista que nos envio el componente subirarchivos
            foreach (var archivo in listadearchivos)
            {
                archivo.FechaDeSubida = DateTime.Now;

                //Obtener la extension del archivo - tipo de documento
                string extencion = archivo.NombreArchivo.ToString().Split('.').Last();
                archivo.ExtensionArchivo = extencion;

                if (extencion == "jpg" || extencion == "jpeg" || extencion == "png")
                {
                    context.Add(archivo);
                    await context.SaveChangesAsync(user.Id);

                    //despues de que almacenamos los archivos ya podemos obtener los id autoincrementables que nos da la bd, para retornarlos
                    listaARetornar.Add(archivo.ArchivoAdjuntoId);
                }
                else
                {
                    string mensajeError = "Archivo no válido!";
                    return BadRequest(mensajeError);
                }
            }
            return listaARetornar;
        }

        [Route("Mostrarpicture/{Id}")]
        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetPicture(int Id)
        {
            //buscamos en la tabla los archivos con el id envidado
            var archivos = context.ArchivoAdjuntos.Where(x => x.ArchivoAdjuntoId == Id).ToList();

            byte[] salida = new byte[0];//inicilizamos un array vacio de bytes

            if (archivos == null)
            {
                //si no se econtro archivos retornamos notfound
                return NotFound();
            }
            else
            {
                foreach (var item in archivos)
                {
                    salida = item.ArchivoEnBytes;//llenamos el arreglo vacio de bytes con los bytes del archivo obtenido
                }
                return File(salida, "image/jpg");// retornamos los bytes transformados en lectura a jpg                
            }
        }

        //////////////////////////////////////////////////
        // APIS PARA SUBIR MULTIPLES VIDEOS//////////////
        /////////////////////////////////////////////////   

        [Route("Videos")]
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrador,Usuario")]
        public async Task<ActionResult<List<int>>> PostVideos(List<ArchivoAdjunto> listadearchivos)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            //lista a devolver con los id de los archivosadjuntos
            List<int> listaARetornar = new List<int>();

            //recorremos la lista que nos envio el componente subirarchivos
            foreach (var archivo in listadearchivos)
            {
                archivo.FechaDeSubida = DateTime.Now;

                //Obtener la extension del archivo - tipo de documento
                string extencion = archivo.NombreArchivo.ToString().Split('.').Last();
                archivo.ExtensionArchivo = extencion;

                if (extencion == "mp4")
                {
                    string nombreContenedor = "VideoServer";
                    var path = $"{environment.WebRootPath}\\{nombreContenedor}\\{archivo.NombreArchivo}";

                    var fs = System.IO.File.Create(path);
                    fs.Write(archivo.ArchivoEnBytes, 0, archivo.ArchivoEnBytes.Length);
                    fs.Close();

                    //pathbase es para obtener la url base en este caso capacitate solo cuando esta en IIS en local biene vacio no afecta
                    var urlActual = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}{httpContextAccessor.HttpContext.Request.PathBase}";
                    var rutaParaBD = Path.Combine(urlActual, nombreContenedor, archivo.NombreArchivo);

                    archivo.UrlLocal = rutaParaBD;
                    archivo.ArchivoEnBytes = null;//vienen bytes pero se hacen null ya que causa error si se almacenan 

                    context.Add(archivo);
                    await context.SaveChangesAsync(user.Id);

                    //despues de que almacenamos los archivos ya podemos obtener los id autoincrementables que nos da la bd, para retornarlos
                    listaARetornar.Add(archivo.ArchivoAdjuntoId);
                }
                else
                {
                    string mensajeError = "Archivo no válido!";
                    return BadRequest(mensajeError);
                }
            }
            return listaARetornar;
        }

        [Route("ObtenerListadodeVideos")]
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrador,Usuario")]
        public async Task<ActionResult<List<ArchivoAdjunto>>> PostVideos(List<VideosCapacitacion> listadevideos)
        {
            List<ArchivoAdjunto> listadearchivos = new List<ArchivoAdjunto>();
            foreach(var item in listadevideos)
            {
                var archivo = context.ArchivoAdjuntos.Where(x => x.ArchivoAdjuntoId == item.ArchivoId).FirstOrDefault();
                if (archivo != null){

                    listadearchivos.Add(archivo);
                }
            }
           
            return listadearchivos;
        }

        #region VERLOSVIDEOSPORMEDIODEBYTES
        //SI SE QUIERE VER LOS VIDEOS POR MEDIO DE BYTES DESCOMENTAR
        //[Route("Videos")]
        //[HttpPost]
        //public async Task<ActionResult<List<int>>> PostVideos(List<ArchivoAdjunto> listadearchivos)
        //{
        //    var user = await _userManager.GetUserAsync(HttpContext.User);

        //    //lista a devolver con los id de los archivosadjuntos
        //    List<int> listaARetornar = new List<int>();

        //    //recorremos la lista que nos envio el componente subirarchivos
        //    foreach (var archivo in listadearchivos)
        //    {
        //        archivo.FechaDeSubida = DateTime.Now;

        //        //Obtener la extension del archivo - tipo de documento
        //        string extencion = archivo.NombreArchivo.ToString().Split('.').Last();
        //        archivo.ExtensionArchivo = extencion;

        //        if (extencion == "mp4")
        //        {
        //            context.Add(archivo);
        //            await context.SaveChangesAsync(user.Id);

        //            //despues de que almacenamos los archivos ya podemos obtener los id autoincrementables que nos da la bd, para retornarlos
        //            listaARetornar.Add(archivo.ArchivoAdjuntoId);
        //        }
        //        else
        //        {
        //            string mensajeError = "Archivo no válido!";
        //            return BadRequest(mensajeError);
        //        }
        //    }
        //    return listaARetornar;
        //}
        #endregion

        #region YaNoSeUsa
        //[HttpPost("{id}")]
        //public async Task Post(int Id)
        //{           
        //    //Comprueba si se cargó algún archivo 
        //    if (HttpContext.Request.Form.Files.Any())
        //    {
        //        foreach (var file in HttpContext.Request.Form.Files)
        //        {                    
        //            //si usas wwwroot/Uploads necesitas WebRootPath
        //            //si no usas /Uploads necesitas ContentRootPath
        //            var path = Path.Combine(environment.WebRootPath, "Uploads", file.FileName);                   

        //            //creamos un FileStream usando el método FileMode.Create()
        //            using (var stream = new FileStream(path, FileMode.Create))
        //            {
        //                //escribimos el byte[] en el archivo usando el método Copy()
        //                await file.CopyToAsync(stream);                     
        //            }

        //            //buscamos el proveedor en la bd que sea igual al proveedor enviado
        //            var proveedor = context.Proveedores.SingleOrDefault(x => x.ProveeedorId == Id);

        //            using (var ms = new MemoryStream())//inicializamos el memorystream
        //            {
        //                await file.CopyToAsync(ms);//copiamos el file que viene del foreach al memorystream
        //                proveedor.ArchivoAdjunto = ms.ToArray();//convertimos el memorystream en array
        //                proveedor.NombreArchivo = file.FileName;
        //            }                    

        //            context.Proveedores.Update(proveedor);
        //            await context.SaveChangesAsync();
        //        }
        //    }
        //}

        #endregion
    }
}
