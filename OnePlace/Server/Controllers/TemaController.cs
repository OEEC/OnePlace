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
using System.Threading.Tasks;

namespace OnePlace.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrador,Usuario")]
    public class TemaController : ControllerBase
    {
        private readonly oneplaceContext context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAlmacenadorArchivos almacenadorDeArchivos;
        private readonly IMapper mapper;
        public TemaController(oneplaceContext context, UserManager<ApplicationUser> userManager, IAlmacenadorArchivos almacenadorDeArchivos, IMapper mapper)
        {
            this.context = context;
            _userManager = userManager;
            this.almacenadorDeArchivos = almacenadorDeArchivos;
            this.mapper = mapper;
        }

        [HttpPost]
        [DisableRequestSizeLimit]//permite subir archivos superiores a 30000 30MB
        //[RequestSizeLimit(52428800)]//limite 50MB
        [RequestSizeLimit(524288000)]//limite 500MB
        public async Task<ActionResult<int>> Post(Tema tema)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            //guardamos el tema
            tema.Activo = true;
            context.Add(tema);
            await context.SaveChangesAsync(user.Id);

            //traemos un listado de fases
            var listadodefases = await context.FaseCursos.ToListAsync();

            //recorremos el listado de fases y por cada fase se va a crear un objeto de tipo TemaFase, con el fin de que cada tema tenga varias fases
            foreach(var item in listadodefases)
            {
                TemaFase temaFase = new TemaFase();
                temaFase.TemaId = tema.TemaId;
                temaFase.FaseCursoId = item.FaseCursoId;

                temaFase.Tema = null;
                temaFase.FaseCurso = null;

                context.Add(temaFase);
            }

            await context.SaveChangesAsync(user.Id);
            return tema.TemaId;
        }
       
        [HttpGet("{id}")]
        public async Task<ActionResult<Tema>> Get(int id)
        {
            var tema = await context.Temas.Where(x => x.TemaId == id).FirstOrDefaultAsync();
            if (tema == null) { return NotFound(); }

            //si el usuario no subio imagen poner una por defecto
            if (string.IsNullOrEmpty(tema.Imagen))
            {
                // Aquí colocas la URL de la imagen por defecto
                tema.Imagen = "Img" + "/" + "Imagenotfound.jpg";
            }

            return tema;
        }

        [HttpPut]
        public async Task<ActionResult> Put(Tema tema)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var oldtema = await context.Temas.FindAsync(tema.TemaId);

            if (tema.Curso != null)
            {
                tema.CursoId = tema.Curso.CursoId;
                tema.Curso = null;
            }

            if (string.IsNullOrWhiteSpace(tema.Imagen))
            {
                tema.Imagen = oldtema.Imagen;
            }

            context.Entry(oldtema).CurrentValues.SetValues(tema);

            await context.SaveChangesAsync(user.Id);
            return NoContent();
        }

        [Route("Desactivar")]
        [HttpPut]
        public async Task<ActionResult> PutDesactivar(Tema tema)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            //obtener el registro original usando el método FindAsync 
            var oldtema = await context.Temas.FindAsync(tema.TemaId);

            //las propiedades sin cambios se ignoran y solo los valores de cambios se incluyen en la consulta de actualización
            context.Entry(oldtema).CurrentValues.SetValues(tema);

            await context.SaveChangesAsync(user.Id);
            return NoContent();
        }

        ///////////////////////////////////
        // APIS PARA FILTROS DE BÚSQUEDA //
        //////////////////////////////////  

        //utilizamos fromquery para traer los parametros de busqueda
        [HttpGet("filtrar/{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Tema>>> Get([FromQuery] ParametrosBusqueda parametrosBusqueda, int id)
        {
            bool mostrar = true;
            //query para traer las facturas
            var queryable = context.Temas.Where(x => x.CursoId == id &&  x.Activo == mostrar).OrderBy(x => x.TemaId).AsQueryable();
                      
            if (parametrosBusqueda.TemaId != 0)
            {
                queryable = queryable.Where(x => x.TemaId == parametrosBusqueda.TemaId);
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
            var temas = await queryable.Paginar(parametrosBusqueda.Paginacion).ToListAsync();
            return temas;
        }
        public class ParametrosBusqueda
        {
            public int Pagina { get; set; } = 1;
            public int CantidadRegistros { get; set; } = 10;
            public PaginacionDTO Paginacion
            {
                get { return new PaginacionDTO() { Pagina = Pagina, CantidadRegistros = CantidadRegistros }; }
            }            
            public int TemaId { get; set; }
            public bool Activo { get; set; }
        }

        [HttpGet("buscar/{textoBusqueda}")]
        public async Task<ActionResult<List<Tema>>> GetTema(string textoBusqueda)
        {
            if (textoBusqueda.Length > 3)
            {
                if (string.IsNullOrWhiteSpace(textoBusqueda)) { return new List<Tema>(); }
                textoBusqueda = textoBusqueda.ToLower();
                return await context.Temas.Where(x => x.Nombre.ToLower().Contains(textoBusqueda)).Take(25).ToListAsync();
            }
            else
            {
                if (string.IsNullOrWhiteSpace(textoBusqueda)) { return new List<Tema>(); }
                textoBusqueda = textoBusqueda.ToLower();
                return await context.Temas.Where(x => x.Nombre.ToLower().Contains(textoBusqueda)).Take(5).ToListAsync();
            }
        }
    }
}
