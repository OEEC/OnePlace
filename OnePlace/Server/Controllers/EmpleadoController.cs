using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnePlace.Server.Data;
using OnePlace.Server.Helpers;
using OnePlace.Shared.DTOs;
using OnePlace.Shared.Entidades.SimsaCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnePlace.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrador")]
    public class EmpleadoController : ControllerBase
    {
        private readonly oneplaceContext context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<EmpleadoController> logger;
        public EmpleadoController(oneplaceContext context, UserManager<ApplicationUser> userManager, ILogger<EmpleadoController> logger)
        {
            this.context = context;
            _userManager = userManager;
            this.logger = logger;
        }

        //[HttpGet]
        //public async Task<ActionResult<List<Empleado>>> Get()
        //{
        //    logger.LogWarning("Log al pedir empleados");

        //    //ejemplo con la excepcion incluida
        //    try
        //    {
        //        throw new NotImplementedException();
        //    }
        //    catch (NotImplementedException ex)
        //    {
        //        logger.LogError(ex, ex.Message);
        //    }

        //    var user = await _userManager.GetUserAsync(HttpContext.User);
        //    logger.LogError("{UserId} some message", user.Id);

        //    var listadodeempleados = await context.Empleados.ToListAsync();
        //    return listadodeempleados;
        //}

        [HttpPost]
        public async Task<ActionResult<int>> Post(Empleado empleado)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            context.Add(empleado);
            await context.SaveChangesAsync(user.Id);
            return empleado.Idempleado;
        }

        [HttpGet]
        public async Task<ActionResult<List<Empleado>>> GetEmp([FromQuery] PaginacionDTO paginacion)
        {    
            //para obtener una union de dos tablas que no estan unidas por fk, se realiza una consulta linq
            //se crea un nuevo objeto empleado pero se tiene que poner toda su data y como propiedad de navegacion
            //se hace otra consulta para obtener la data persona por que no se puede usar el include
            List<Empleado> empleados = (from e in context.Empleados
                                       select new Empleado
                                       {
                                           Idempleado = e.Idempleado,
                                           Idpersona = e.Idpersona,
                                           Img = e.Img,
                                           Noemp = e.Noemp,
                                           Correo = e.Correo,
                                           Telefono = e.Telefono,
                                           Iddepartamento = e.Iddepartamento,
                                           Idarea = e.Idarea,
                                           Idpuesto = e.Idpuesto,
                                           Nomina = e.Nomina,
                                           Variable = e.Variable,
                                           Idtipo = e.Idtipo,
                                           Fchalta = e.Fchalta,
                                           Fchbaja = e.Fchbaja,
                                           Persona = context.Personas.Where(x => x.Idpersona == e.Idpersona).FirstOrDefault()
                                       }).ToList();

            foreach (var item in empleados)
            {
                if (string.IsNullOrEmpty(item.Img))
                {
                    // Aquí colocas la URL de la imagen por defecto
                    item.Img = "Img" + "/" + "usuario.png";
                }
            }

            var queryable = empleados.AsQueryable();
            await HttpContext.InsertarParametrosPaginacionEnRespuesta(queryable, paginacion.CantidadRegistros,true);
            return queryable.Paginar(paginacion).ToList();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EmpleadoPersonaDTO>> Get(int id)
        {
            var empleado = await context.Empleados.Where(x => x.Idempleado == id).FirstOrDefaultAsync();

            if (empleado == null) { return NotFound(); }

            //si el usuario no subio imagen poner una por defecto
            if (string.IsNullOrEmpty(empleado.Img))
            {
                // Aquí colocas la URL de la imagen por defecto
                empleado.Img = "Img" + "/" + "avatars-1.png";
            }

            var persona = await context.Personas.Where(x => x.Idpersona == empleado.Idpersona).FirstOrDefaultAsync();
            if (persona == null) { return NotFound(); }           

            var model = new EmpleadoPersonaDTO();
            model.Empleado = empleado;
            model.Persona = persona;

            return model;
        }
    }
}
