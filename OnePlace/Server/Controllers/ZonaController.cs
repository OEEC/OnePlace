using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnePlace.Server.Data;
using OnePlace.Shared.DTOs.Modelos;
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
        private readonly ILogger<ZonaController> logger;
        private readonly IMapper mapper;

        public ZonaController(oneplaceContext context, ILogger<ZonaController> logger, IMapper mapper)
        {
            this.context = context;
            this.logger = logger;
            this.mapper = mapper;
        }

        //obtener zonas por medio de filtros en base al dto de zona
        [HttpGet]
        public async Task<ActionResult<List<ZonaDTO>>> Get([FromQuery] ZonaDTO zona)
        {
            var zonas = context.Zonas
                .AsNoTracking()
                .Where(x => !string.IsNullOrEmpty(x.Zona1) && x.Idestatus == 1)
                .OrderBy(x => x.Zona1)
                .AsQueryable();

            if (!string.IsNullOrEmpty(zona.Zona) && !string.IsNullOrWhiteSpace(zona.Zona))
                zonas = zonas.Where(x => x.Zona1.ToLower().StartsWith(zona.Zona.ToLower()));

            return Ok(zonas.Select(mapper.Map<ZonaDTO>));
        }
        //buscar deptos para filtro
        [HttpGet("zonas")]
        public async Task<ActionResult<List<Zona>>> GetZona()
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
