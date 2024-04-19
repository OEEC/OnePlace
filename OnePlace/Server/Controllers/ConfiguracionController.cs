using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnePlace.Client.ComponentesGenericos.Imagen;
using OnePlace.Server.Data;
using OnePlace.Shared.Entidades;
using OnePlace.Shared.Enums;
using System;
using System.Linq;
using System.Threading.Tasks;

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
                var color = context.Configuracion.FirstOrDefault(x => x.Tipo == Tipo_Configuracion.Color_Fondo_Header.ToString() && x.Estatus);
                if (color is null) { return Ok(new Configuracion()); }
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
                var color = context.Configuracion.FirstOrDefault(x => x.Tipo == Tipo_Configuracion.Color_Fondo_Footer.ToString() && x.Estatus);
                if (color is null) { return Ok(new Configuracion()); }
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
                var color = context.Configuracion.FirstOrDefault(x => x.Tipo == Tipo_Configuracion.Color_Fondo_Carousel.ToString() && x.Estatus);
                if (color is null) { return Ok(new Configuracion()); }
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
                var texto = context.Configuracion.FirstOrDefault(x => x.Tipo == Tipo_Configuracion.Texto_Portada.ToString() && x.Estatus);
                if (texto is null) { return Ok(new Configuracion()); }
                return Ok(texto);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("imagen/logo")]
        public ActionResult Obtener_Imagen_Logo()
        {
            try
            {
                var imagen = context.Configuracion.FirstOrDefault(x => x.Tipo == Tipo_Configuracion.Imagen_Logo.ToString() && x.Estatus);
                if (imagen is null) { return Ok(new Configuracion()); }
                return Ok(imagen);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("imagen/portada")]
        public ActionResult Obtener_Imagen_Portada()
        {
            try
            {
                var imagen = context.Configuracion.FirstOrDefault(x => x.Tipo == Tipo_Configuracion.Imagen_Portada.ToString() && x.Estatus);
                if (imagen is null) { return Ok(new Configuracion()); }
                return Ok(imagen);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("fondo/color/header")]
        public async Task Guardar_Configuracion_Color_Fondo_Superior([FromBody] Configuracion configuracion)
        {
            if(configuracion.Id == 0)
            {
                configuracion.Tipo = Tipo_Configuracion.Color_Fondo_Header.ToString();
                context.Add(configuracion);
                await context.SaveChangesAsync();
            }
            else
            {
                context.Update(configuracion);
                await context.SaveChangesAsync();
            }
        }

        [HttpPost("fondo/color/carousel")]
        public async Task Guardar_Configuracion_Color_Fondo_Central([FromBody] Configuracion configuracion)
        {
            if (configuracion.Id == 0)
            {
                configuracion.Tipo = Tipo_Configuracion.Color_Fondo_Carousel.ToString();
                context.Add(configuracion);
                await context.SaveChangesAsync();
            }
            else
            {
                context.Update(configuracion);
                await context.SaveChangesAsync();
            }
        }

        [HttpPost("fondo/color/footer")]
        public async Task Guardar_Configuracion_Color_Fondo_Footer([FromBody] Configuracion configuracion)
        {
            if (configuracion.Id == 0)
            {
                configuracion.Tipo = Tipo_Configuracion.Color_Fondo_Footer.ToString();
                context.Add(configuracion);
                await context.SaveChangesAsync();
            }
            else
            {
                context.Update(configuracion);
                await context.SaveChangesAsync();
            }
        }

        [HttpPost("texto/portada")]
        public async Task Guardar_Configuracion_Texto([FromBody] Configuracion configuracion)
        {
            if (configuracion.Id == 0)
            {
                configuracion.Tipo = Tipo_Configuracion.Texto_Portada.ToString();
                context.Add(configuracion);
                await context.SaveChangesAsync();
            }
            else
            {
                context.Update(configuracion);
                await context.SaveChangesAsync();
            }
        }

        [HttpPost("imagen/logo")]
        public async Task Guardar_Configuracion_Imagen_Logo([FromBody] Configuracion configuracion)
        {
            if (configuracion.Id == 0)
            {
                configuracion.Tipo = Tipo_Configuracion.Imagen_Logo.ToString();
                context.Add(configuracion);
                await context.SaveChangesAsync();
            }
            else
            {
                context.Update(configuracion);
                await context.SaveChangesAsync();
            }
        }

        [HttpPost("imagen/portada")]
        public async Task Guardar_Configuracion_Imagen_Portada([FromBody] Configuracion configuracion)
        {
            if (configuracion.Id == 0)
            {
                configuracion.Tipo = Tipo_Configuracion.Imagen_Portada.ToString();
                context.Add(configuracion);
                await context.SaveChangesAsync();
            }
            else
            {
                context.Update(configuracion);
                await context.SaveChangesAsync();
            }
        }
    }
}
