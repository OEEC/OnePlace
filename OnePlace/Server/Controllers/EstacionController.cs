using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnePlace.Server.Data;
using OnePlace.Shared.DTOs.Modelos;
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
    public class EstacionController : ControllerBase
    {
        private readonly oneplaceContext context;
        private readonly IMapper mapper;

        public EstacionController(oneplaceContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        //obtener estaciones por medio de filtros en base al dto de estacion
        [HttpGet()]
        public async Task<ActionResult<List<EstacionDTO>>> Get([FromQuery] EstacionDTO estacion)
        {
            var estaciones = context.Estaciones
                .AsNoTracking()
                .Where(x => !string.IsNullOrEmpty(x.Nombre) && x.Estatus == 1)
                .OrderBy(x => x.Nombre)
                .AsQueryable();

            if (!string.IsNullOrEmpty(estacion.Nombre) && !string.IsNullOrWhiteSpace(estacion.Nombre))
                estaciones = estaciones.Where(x => x.Nombre.ToLower().StartsWith(estacion.Nombre.ToLower()));

            return Ok(estaciones.Select(mapper.Map<EstacionDTO>));
        }


        [HttpGet("buscar/{textoBusqueda}")]
        public async Task<ActionResult<List<Estacion>>> GetEstacion(string textoBusqueda)
        {
            if (textoBusqueda.Length > 3)
            {
                if (string.IsNullOrWhiteSpace(textoBusqueda)) { return new List<Estacion>(); }
                textoBusqueda = textoBusqueda.ToLower();
                return await context.Estaciones.Where(x => x.Nombre.ToLower().Contains(textoBusqueda)).ToListAsync();
            }
            else
            {
                if (string.IsNullOrWhiteSpace(textoBusqueda)) { return new List<Estacion>(); }
                textoBusqueda = textoBusqueda.ToLower();
                return await context.Estaciones.Where(x => x.Nombre.ToLower().Contains(textoBusqueda)).Take(50).ToListAsync();
            }
        }
    }
}
