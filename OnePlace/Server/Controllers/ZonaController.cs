using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnePlace.Server.Data;
using OnePlace.Shared.Entidades.SimsaCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnePlace.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ZonaController : ControllerBase
    {
        private readonly oneplaceContext context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ZonaController> logger;
        public ZonaController(oneplaceContext context, UserManager<ApplicationUser> userManager, ILogger<ZonaController> logger)
        {
            this.context = context;
            _userManager = userManager;
            this.logger = logger;
        }

        //buscar deptos para filtro
        [HttpGet("zonas")]
        public async Task <ActionResult<List<Zona>>> GetZona()
        {
           var zonas = await context.Zonas.ToListAsync();
            return Ok(zonas);
        }

        [HttpGet("zonas/{id}")]
        public async Task<ActionResult<Zona>> GetEmpleadoZona(int id)  
        {
            var zona = await context.Zonas.Where(x => x.ZonaId == id).FirstOrDefaultAsync();
            return Ok(zona);
        }
    }
}
