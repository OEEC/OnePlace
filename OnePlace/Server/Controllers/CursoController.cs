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

            if(curso.FechaInicio == null)
            {
                string mensajeError = "Ingrese una fecha de Inicio";
                return BadRequest(mensajeError);
            }          

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

        //endpoints para cursos en la pagina de inicio
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

        [Route("DetalleCurso/{id}")]
        [HttpGet]
        public async Task<ActionResult<Curso>> GetDetalle(int id)
        {
            var fechadehoy = DateTime.Today;
            //solo traer los cursos que no estan vencidos, esto evita que lleguen al curso por url, aun y cuando ya no se muestran en pantalla
            var curso = await context.Cursos.Where(x => x.CursoId == id && fechadehoy <= x.FechaFinal)
               .Include(x => x.LisadeTemas).FirstOrDefaultAsync();
            if (curso == null) { return NotFound(); }
            return curso;
        }       

        [Route("ReactivarCurso")]
        [HttpPost]
        public async Task<ActionResult> PostReactivarCurso(CursoEstado cursoestado)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            #region ActividadQuiz

            //obtenemos una lista con todas las actividades de usuario quiz que correspondal al id del empleado enviado por parametro
            var listaactividadquiz = await context.ActividadUsuarioQuiz
                .Where(x => x.Idempleado == cursoestado.Idempleado).ToListAsync();

            List<EstadosdelQuiz> listadodeestadodelquiz = new List<EstadosdelQuiz>();

            //recorremos el listadodeactividadesquiz para obtener un listado filtrado unicamente por los quiz reprobados
            foreach(var actividad in listaactividadquiz)
            {
                var estadoquiz = await context.EstadosdelQuiz
                    .Where(x => x.EstadosdelQuizId == actividad.EstadosdelQuizId && x.Evaluacion == Evaluacion.Reprobado).FirstOrDefaultAsync();

                if(estadoquiz != null)
                {
                    listadodeestadodelquiz.Add(estadoquiz);
                }               
            }

            List<ActividadUsuarioQuiz> listactividadesquizfiltradaporreprobado= new List<ActividadUsuarioQuiz>();

            //una vez que tenemos los quiz reprobados volvemos a obtener una lista de actividades quiz pero ya reducida a los quiz reprobados
            foreach (var statusdelquiz in listadodeestadodelquiz)
            {
                var actividad = await context.ActividadUsuarioQuiz
                    .Where(x => x.EstadosdelQuizId == statusdelquiz.EstadosdelQuizId).FirstOrDefaultAsync();

                if(actividad != null)
                {
                    listactividadesquizfiltradaporreprobado.Add(actividad);
                }              
            }

            //al final eliminamos los registros de listado de actividades usuario quiz y estado quiz donde hayan sido reprobados
            //primero eliminamos las actividades y luego los estados si se hace al reves causa error

            context.RemoveRange(listactividadesquizfiltradaporreprobado);
            await context.SaveChangesAsync(user.Id);

            context.RemoveRange(listadodeestadodelquiz);
            await context.SaveChangesAsync(user.Id);           

            #endregion

            #region ActividadUsuario

            //obtenemos un listado de todas las actividades de usuario filtradas por el id del empleado enviado por parametro
            var listadeactividades = await context.ActividadUsuarios
                .Where(x => x.Idempleado == cursoestado.Idempleado).ToListAsync();

            //obtenemos un listado de temas filtrado por el id del curso enviado por parametro
            var listadetemas = await context.Temas
                .Where(x => x.CursoId == cursoestado.CursoId).ToListAsync();

            List<ActividadUsuario> listadeactividadesretornar = new List<ActividadUsuario>();

            //obtenemos una lista de actividades de usuario pero ya filtrada unicamente por las actividades que pertenecen a los temas 
            //que a su vez pertenecen al curso seleccionado, cremos una lista comparando dos listas y unificando los id coincidentes
            listadeactividadesretornar = listadeactividades.Where(p => listadetemas.Any(p2 => p2.TemaId == p.TemaId)).ToList();

            context.RemoveRange(listadeactividadesretornar);
            await context.SaveChangesAsync(user.Id);

            #endregion          

            #region Respuestas

            //obtenemos una lista de quiz, reccoriendo un listado de temas los cuales pertenecen al curso seleccionado
            List<Quiz> listadequizzes = new List<Quiz>();
            foreach(var tema in listadetemas)
            {
                var quiz = await context.Quizzes.Where(x => x.TemaId == tema.TemaId).Include(x=>x.LisadePreguntas).FirstOrDefaultAsync();
                if(quiz != null)
                {
                    listadequizzes.Add(quiz);
                }                
            }

            //recorremos el listado de quizzes y luego recorremos cada pregunta de cada quiz, para obtener un listado de preguntas que pertenezcan a ese quiz 
            
            List<Pregunta> listadepreguntas = new List<Pregunta>();
            foreach (var quiz in listadequizzes)
            {
                foreach(var item in quiz.LisadePreguntas)
                {
                    var pregunta = await context.Preguntas.Where(x => x.PreguntaId == item.PreguntaId).FirstOrDefaultAsync();
                    if(pregunta != null)
                    {
                        listadepreguntas.Add(pregunta);
                    }                  
                }                
            }

            //recorremos el listado de preguntas para obtener un listado de respuestas que pertenezcan a esas preguntas
            List<Respuesta> listaderespuestas = new List<Respuesta>();
            foreach (var item in listadepreguntas)
            {
                var respuesta = await context.Respuestas.Where(x => x.PreguntaId == item.PreguntaId).FirstOrDefaultAsync();
                if(respuesta != null)
                {
                    listaderespuestas.Add(respuesta);
                }              
            }

            //borramos las respuestas
            context.RemoveRange(listaderespuestas);
            await context.SaveChangesAsync(user.Id);

            #endregion

            #region CursoEstado

            //obtenemos el registro de cursoestado donde sea igual al cursoid & empleadoid enviado por parametro y que su estado sea sin completar
            var CursoEstado = await context.CursoEstado
           .Where(x => x.CursoId == cursoestado.CursoId
            && x.Idempleado == cursoestado.Idempleado
            && x.EstadoCurso == EstadoCurso.SinCompletar)
           .FirstOrDefaultAsync();

            context.Remove(CursoEstado);
            await context.SaveChangesAsync(user.Id);

            #endregion

            return Ok();
        }
    }
}
