using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnePlace.Server.Data;
using OnePlace.Shared.Entidades;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OnePlace.Shared.IdentityModels;
using System.Linq;
using OnePlace.Shared.DTOs;

namespace OnePlace.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GrupoController : ControllerBase
    {
        private readonly oneplaceContext context;
        private readonly UserManager<IdentityUsuario> userManager;

        public GrupoController(oneplaceContext context, UserManager<IdentityUsuario> userManager)
        {
            this.context = context;
            this.userManager = userManager;
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

        [HttpGet("departamento")]
        public async Task<ActionResult> Departamento()
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var empleado = await context.Empleados.FirstAsync(e => e.Idempleado == user.Idempleado);

                // Variable para almacenar la información a retornar
                object resultado;

                if (empleado.Division == "ADMINISTRATIVO")
                {
                    resultado = await context.Departamentos
                        .Where(d => d.Iddepartamento == empleado.Iddepartamento)
                        .Select(d => new QNavbarDTO
                        {
                            Division = empleado.Division,
                            AreaTrabajo = d.Nombre_Departamento
                        })
                        .FirstOrDefaultAsync();
                }
                else
                {
                    resultado = await context.Estaciones
                        .Where(a => a.Idestacion == empleado.Idestacion)
                        .Select(a => new QNavbarDTO {
                            Division = empleado.Division,
                            AreaTrabajo = a.Nombre
                        })
                        .FirstOrDefaultAsync();
                }

                return Ok(resultado);
            }
            catch (Exception e)
            {
                // Devuelve un mensaje de error si ocurre una excepción
                return BadRequest(new { mensaje = e.Message });
            }
        }
    }
}
