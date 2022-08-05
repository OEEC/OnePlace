using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnePlace.Server.Data;
using OnePlace.Shared.Entidades.SimsaCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnePlace.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpleadoController : ControllerBase
    {
        private readonly oneplaceContext context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<EmpleadoController> logger;
        public EmpleadoController(oneplaceContext context,UserManager<ApplicationUser> userManager,ILogger<EmpleadoController> logger)
        {
            this.context = context;
            _userManager = userManager;
            this.logger = logger;
        }
        
        [HttpGet]
        public async Task<ActionResult<List<Empleado>>> Get()
        {
            logger.LogWarning("Log al pedir empleados");

            //ejemplo con la excepcion incluida
            try
            {
                throw new NotImplementedException();
            }
            catch (NotImplementedException ex)
            {
                logger.LogError(ex, ex.Message);
            }

            var user = await _userManager.GetUserAsync(HttpContext.User);
            logger.LogError("{UserId} some message", user.Id);

            var listadodeempleados = await context.Empleados.ToListAsync();  
            return listadodeempleados;
        }

        [HttpPost]
        public async Task<ActionResult<int>> Post(Empleado empleado)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);          
            context.Add(empleado);
            await context.SaveChangesAsync(user.Id);           
            return empleado.Idempleado;
        }
    }
}
