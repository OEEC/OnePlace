using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnePlace.Server.Data;
using OnePlace.Shared.Entidades;
using System;
using System.Threading.Tasks;

namespace OnePlace.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GrupoController : ControllerBase
    {
        private readonly oneplaceContext context;

        public GrupoController(oneplaceContext context)
        {
            this.context = context;
        }

        [HttpGet("grupos")]
        public async Task<ActionResult> Grupos()
        {
            try
            {
                // Obtiene todos los grupos de forma asincrónica
                var grupos = await context.QGrupo.ToArrayAsync();
                return Ok(grupos);
            }
            catch (Exception e)
            {
                // Devuelve un mensaje de error si ocurre una excepción
                return BadRequest(new { mensaje = e.Message });
            }
        }
    }
}
