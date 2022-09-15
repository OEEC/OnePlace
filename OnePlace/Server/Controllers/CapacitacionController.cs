using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnePlace.Server.Data;
using OnePlace.Server.Helpers;
using OnePlace.Shared.DTOs;
using OnePlace.Shared.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace OnePlace.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrador,Usuario")]
    public class CapacitacionController : ControllerBase
    {
        private readonly oneplaceContext context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper mapper;
        public CapacitacionController(oneplaceContext context, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            this.context = context;
            _userManager = userManager;
            this.mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<int>> Post(CapacitacionContinua capacitacion)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            capacitacion.Activo = true;
            context.Add(capacitacion);
            await context.SaveChangesAsync(user.Id);          

            return capacitacion.CapacitacionContinuaId;
        }

        //utilizamos fromquery para traer los parametros de busqueda
        [HttpGet("filtrar")]
        [AllowAnonymous]
        public async Task<ActionResult<List<CapacitacionContinua>>> Get([FromQuery] ParametrosBusqueda parametrosBusqueda)
        {
            bool mostrar = true;
            //query para traer los eventos
            var queryable = context.CapacitacionContinua.Where(x => x.Activo == mostrar).OrderBy(x => x.CapacitacionContinuaId).AsQueryable();

            if (parametrosBusqueda.CapacitacionId != 0)
            {
                queryable = queryable.Where(x => x.CapacitacionContinuaId == parametrosBusqueda.CapacitacionId);
            }
            if (parametrosBusqueda.Activo == true)
            {
                //queryable = queryable.Where(x => x.Activo == false);
                mostrar = false;
            }

            //paginacion
            await HttpContext.InsertarParametrosPaginacionEnRespuesta(queryable, parametrosBusqueda.CantidadRegistros);
            var eventos = await queryable.Paginar(parametrosBusqueda.Paginacion).ToListAsync();
            return eventos;
        }
        public class ParametrosBusqueda
        {
            public int Pagina { get; set; } = 1;
            public int CantidadRegistros { get; set; } = 10;
            public PaginacionDTO Paginacion
            {
                get { return new PaginacionDTO() { Pagina = Pagina, CantidadRegistros = CantidadRegistros }; }
            }
            public int CapacitacionId { get; set; }
            public bool Activo { get; set; }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CapacitacionContinua>> Get(int id)
        {
            var capacitacion = await context.CapacitacionContinua
               .Where(x => x.CapacitacionContinuaId == id)
               .Include(x => x.ListadeVideos)
               .FirstOrDefaultAsync();
            if (capacitacion == null) { return NotFound(); }

            //si el usuario no subio imagen poner una por defecto
            if (string.IsNullOrEmpty(capacitacion.Imagen))
            {
                // Aquí colocas la URL de la imagen por defecto
                capacitacion.Imagen = "Img" + "/" + "Imagenotfound.jpg";
            }

            return capacitacion;
        }

        [HttpPut]
        public async Task<ActionResult> Put(CapacitacionContinua capacitacion)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
           
            //buscamos un tema en la bd por el id del tema
            var CapacitacionDB = await context.CapacitacionContinua.FirstOrDefaultAsync(x => x.CapacitacionContinuaId == capacitacion.CapacitacionContinuaId);

            if (CapacitacionDB == null) { return NotFound(); }

            //si el tema de la bd no tiene imagen le ponemos la nueva que viene por parametro dentro del objeto
            if (!string.IsNullOrWhiteSpace(capacitacion.Imagen))
            {
                CapacitacionDB.Imagen = capacitacion.Imagen;
            }

            //mapeamos el tema de la bd con el que viene como paramtro
            CapacitacionDB = mapper.Map(capacitacion, CapacitacionDB);

            await context.SaveChangesAsync(user.Id);
            return NoContent();
        }

        //buscar facturas para asignarla a un equipo
        [HttpGet("buscar/{textoBusqueda}")]
        public async Task<ActionResult<List<CapacitacionContinua>>> GetCapacitacion(string textoBusqueda)
        {
            if (textoBusqueda.Length > 3)
            {
                if (string.IsNullOrWhiteSpace(textoBusqueda)) { return new List<CapacitacionContinua>(); }
                textoBusqueda = textoBusqueda.ToLower();
                return await context.CapacitacionContinua.Where(x => x.Nombre.ToLower().Contains(textoBusqueda)).ToListAsync();
            }
            else
            {
                if (string.IsNullOrWhiteSpace(textoBusqueda)) { return new List<CapacitacionContinua>(); }
                textoBusqueda = textoBusqueda.ToLower();
                return await context.CapacitacionContinua.Where(x => x.Nombre.ToLower().Contains(textoBusqueda)).Take(50).ToListAsync();
            }
        }

        [Route("Desactivar")]
        [HttpPut]
        public async Task<ActionResult> PutDesactivar(CapacitacionContinua capacitacion)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            //obtener el registro original usando el método FindAsync 
            var oldcapacitacion = await context.CapacitacionContinua.FindAsync(capacitacion.CapacitacionContinuaId);

            //las propiedades sin cambios se ignoran y solo los valores de cambios se incluyen en la consulta de actualización
            context.Entry(oldcapacitacion).CurrentValues.SetValues(capacitacion);

            await context.SaveChangesAsync(user.Id);
            return NoContent();
        }

        [HttpDelete("{id}")]       
        public async Task<ActionResult> Delete(int id)
        {            
            //obtenemos el archivo con el id enviado por parametro            
            var archivo = await context.VideosCapacitacion.Where(x => x.ArchivoId == id).FirstOrDefaultAsync();           
            if(archivo != null)
            {
                context.Remove(archivo);//si es diferente de nulo removemos, el registro con ese id
                await context.SaveChangesAsync();
            }            
            return NoContent();
        }
    }
}
