using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnePlace.Server.Data;
using System;
using System.Linq;

namespace OnePlace.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfiguracionController : ControllerBase
    {
        private readonly oneplaceContext context;

        public ConfiguracionController(oneplaceContext context)
        {
            this.context = context;
        }

        [HttpGet("fondo/color/header")]
        public ActionResult Obtener_Color_Fondo_Header()
        {
            try
            {
                var color = context.Configuracion.FirstOrDefault(x => x.Tipo == "Color_Fondo_Header" && x.Estatus);

                return Ok(color);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("fondo/color/footer")]
        public ActionResult Obtener_Color_Fondo_Footer()
        {
            try
            {
                var color = context.Configuracion.FirstOrDefault(x => x.Tipo == "Color_Fondo_footer" && x.Estatus);

                return Ok(color);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("fondo/color/carousel")]
        public ActionResult Obtener_Color_Fondo_Carousel()
        {
            try
            {
                var color = context.Configuracion.FirstOrDefault(x => x.Tipo == "Color_Fondo_Carousel" && x.Estatus);

                return Ok(color);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("texto/portada")]
        public ActionResult Obtener_Texto_Portada()
        {
            try
            {
                var texto = context.Configuracion.FirstOrDefault(x => x.Tipo == "Texto_Portada" && x.Estatus);

                return Ok(texto);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

    }
}
