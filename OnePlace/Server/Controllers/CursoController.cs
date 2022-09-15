using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnePlace.Shared.DTOs;
using OnePlace.Shared.Entidades;
using OnePlace.Server.Data;
using OnePlace.Server.Helpers;

namespace OnePlace.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrador,Usuario")]
    public class CursoController : ControllerBase
    {
        private readonly oneplaceContext context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAlmacenadorArchivos almacenadorDeArchivos;
        private readonly IMapper mapper;
        public CursoController(oneplaceContext context, UserManager<ApplicationUser> userManager, IAlmacenadorArchivos almacenadorDeArchivos, IMapper mapper)
        {
            this.context = context;
            _userManager = userManager;
            this.almacenadorDeArchivos = almacenadorDeArchivos;
            this.mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<int>> Post(Curso curso)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            curso.Activo = true;

            context.Add(curso);
            await context.SaveChangesAsync(user.Id);
            return curso.CursoId;
        }     

        [HttpGet("{id}")]
        public async Task<ActionResult<Curso>> Get(int id)
        {
            var curso = await context.Cursos.Where(x => x.CursoId == id)
               .Include(x => x.LisadeTemas).FirstOrDefaultAsync();
            if (curso == null) { return NotFound(); }
            return curso;
        }

        [HttpPut]
        public async Task<ActionResult> Put(Curso curso)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var oldcurso = await context.Cursos.FindAsync(curso.CursoId);

            if (string.IsNullOrWhiteSpace(curso.Imagen))
            {
                curso.Imagen = oldcurso.Imagen;
            }

            context.Entry(oldcurso).CurrentValues.SetValues(curso);

            await context.SaveChangesAsync(user.Id);
            return NoContent();
        }

        [Route("Desactivar")]
        [HttpPut]
        public async Task<ActionResult> PutDesactivar(Curso curso)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            //obtener el registro original usando el método FindAsync 
            var oldcurso = await context.Cursos.FindAsync(curso.CursoId);

            //las propiedades sin cambios se ignoran y solo los valores de cambios se incluyen en la consulta de actualización
            context.Entry(oldcurso).CurrentValues.SetValues(curso);

            await context.SaveChangesAsync(user.Id);
            return NoContent();
        }

        ///////////////////////////////////
        // APIS PARA FILTROS DE BÚSQUEDA //
        //////////////////////////////////  

        //utilizamos fromquery para traer los parametros de busqueda
        [HttpGet("filtrar")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Curso>>> Get([FromQuery] ParametrosBusqueda parametrosBusqueda)
        {
            bool mostrar = true;
            //query para traer las facturas
            var queryable = context.Cursos.Where(x => x.Activo == mostrar).OrderBy(x => x.CursoId).AsQueryable();

            if (parametrosBusqueda.CursoId != 0)
            {
                queryable = queryable.Where(x => x.CursoId == parametrosBusqueda.CursoId);
            }
            if (parametrosBusqueda.Activo == true)
            {
                //queryable = queryable.Where(x => x.Activo == false);
                mostrar = false;
            }
            foreach (var item in queryable)
            {
                //si el usuario no subio imagen poner una por defecto
                if (string.IsNullOrEmpty(item.Imagen))
                {
                    // Aquí colocas la URL de la imagen por defecto
                    item.Imagen = "Img" + "/" + "Imagenotfound.jpg";
                }
            }

            //paginacion
            await HttpContext.InsertarParametrosPaginacionEnRespuesta(queryable, parametrosBusqueda.CantidadRegistros);
            var cursos = await queryable.Paginar(parametrosBusqueda.Paginacion).ToListAsync();
            return cursos;
        }
        public class ParametrosBusqueda
        {
            public int Pagina { get; set; } = 1;
            public int CantidadRegistros { get; set; } = 10;
            public PaginacionDTO Paginacion
            {
                get { return new PaginacionDTO() { Pagina = Pagina, CantidadRegistros = CantidadRegistros }; }
            }
            public int CursoId { get; set; }           
            public bool Activo { get; set; }
        }
        
        [HttpGet("buscar/{textoBusqueda}")]
        public async Task<ActionResult<List<Curso>>> GetCurso(string textoBusqueda)
        {
            if (textoBusqueda.Length > 3)
            {
                if (string.IsNullOrWhiteSpace(textoBusqueda)) { return new List<Curso>(); }
                textoBusqueda = textoBusqueda.ToLower();
                return await context.Cursos.Where(x => x.Nombre.ToLower().Contains(textoBusqueda)).ToListAsync();
            }
            else
            {
                if (string.IsNullOrWhiteSpace(textoBusqueda)) { return new List<Curso>(); }
                textoBusqueda = textoBusqueda.ToLower();
                return await context.Cursos.Where(x => x.Nombre.ToLower().Contains(textoBusqueda)).Take(50).ToListAsync();
            }
        }

        //endpoint para cursos en la pagina de inicio
        [HttpGet]
        public async Task<ActionResult<List<Curso>>> Get([FromQuery] PaginacionDTO paginacion)
        {
            var queryable = context.Cursos.Where(x => x.Activo == true).AsQueryable();

            foreach (var item in queryable)
            {
                //si el usuario no subio imagen poner una por defecto
                if (string.IsNullOrEmpty(item.Imagen))
                {
                    // Aquí colocas la URL de la imagen por defecto
                    item.Imagen = "Img" + "/" + "Imagenotfound.jpg";
                }
            }

            await HttpContext.InsertarParametrosPaginacionEnRespuesta(queryable, paginacion.CantidadRegistros);
            return await queryable.Paginar(paginacion).ToListAsync();
        }
    }
}
