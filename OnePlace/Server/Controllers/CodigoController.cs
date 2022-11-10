
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnePlace.Server.Data;
using OnePlace.Server.Helpers;
using OnePlace.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace OnePlace.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrador")]
    public class CodigoController : ControllerBase
    {
        private readonly oneplaceContext context;
        private readonly UserManager<ApplicationUser> _userManager;
        public CodigoController(oneplaceContext context, UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            _userManager = userManager;
        }

        //si retornas el string que mannda el mtodo enerarQR.GenerarCode el json deserailizable no lo lee bien por eso se hizo directamente el llamado a la api 
        //[HttpPost]
        //public async Task<ActionResult<string>> Post(QRcodigo sitioweb)
        //{
        //    var texto = sitioweb.Codigo;
        //    var codigo = GenerarQR.GenerarCode(texto);

        //    //string imageString = codigo.Substring(24);//se le quita data: image/png; base64, para que sea valida la conversion nuevamente a bytes
        //    //byte[] imageBytes = Convert.FromBase64String(imageString);  

        //    return codigo;
        //}

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetQR()
        {
            var texto ="https://innovacion2.dgl.com.mx/oneplace";//contenido del qr
            var codigo = GenerarQR.GenerarCode(texto);//generas el qr

            string imageString = codigo.Substring(24);//se le quita data: image/png; base64, para que sea valida la conversion nuevamente a bytes
            byte[] imageBytes = Convert.FromBase64String(imageString);//lo convertimos en bytes

            byte[] salida = new byte[0];//inicilizamos un array vacio de bytes

            if (codigo == null)
            {
                //si no se econtro archivos retornamos notfound
                return NotFound();
            }
            else
            {
                salida = imageBytes;//llenamos el arreglo vacio de bytes con los bytes del archivo
                return File(salida, "image/jpg");// retornamos los bytes transformados en lectura a jpg                
            }
        }
    }
}
