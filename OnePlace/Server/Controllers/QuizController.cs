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
using OnePlace.Shared.Entidades.SimsaCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OnePlace.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrador,Usuario")]
    public class QuizController : ControllerBase
    {
        private readonly oneplaceContext context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper mapper;
        public QuizController(oneplaceContext context, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            this.context = context;
            _userManager = userManager;
            this.mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Quiz>> Get(int id)
        {
            var quiz = await context.Quizzes
               .Where(x => x.QuizId == id)
               .Include(x => x.LisadePreguntas)
               .FirstOrDefaultAsync();
            if (quiz == null) { return NotFound(); }
            return quiz;
        }

        [Route("Listado/{id}")]
        [HttpGet]
        public async Task<ActionResult<PaginadorGenerico<Pregunta>>> Get(int id, string buscar, Boolean filtro, int pagina, int registros_por_pagina = 10)
        {
            PaginadorGenerico<Pregunta> _PaginadorConceptos;

            var pregunta = await context.Preguntas
                .Where(x => x.Quiz.QuizId == id && x.Activo == true)
                .Include(x => x.PalabrasClave)
                .OrderBy(x => x.PreguntaId)
                .ToListAsync();

            ///////////////////////////
            // SISTEMA DE PAGINACIÓN //
            ///////////////////////////
            ///
            int _TotalRegistros = 0;
            int _TotalPaginas = 0;

            // Número total de registros de la coleccion 
            _TotalRegistros = pregunta.Count();
            // Obtenemos la 'página de registros' de la coleccion 
            pregunta = pregunta.Skip((pagina - 1) * registros_por_pagina)
                                             .Take(registros_por_pagina)
                                             .ToList();
            // Número total de páginas de la coleccion
            _TotalPaginas = (int)Math.Ceiling((double)_TotalRegistros / registros_por_pagina);

            //Instanciamos la 'Clase de paginación' y asignamos los nuevos valores
            _PaginadorConceptos = new PaginadorGenerico<Pregunta>()
            {
                RegistrosPorPagina = registros_por_pagina,
                TotalRegistros = _TotalRegistros,
                TotalPaginas = _TotalPaginas,
                PaginaActual = pagina,

                BusquedaActual = buscar,
                Resultado = pregunta
            };
            return _PaginadorConceptos;
        }

        [Route("Desactivar")]
        [HttpPut]
        public async Task<ActionResult> PutDesactivar(TemaQuizDTO temaquizdto)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
                        
            //si hay una pregunta entra al if para editar el campo activo
            if (temaquizdto.Quiz.LisadePreguntas.Count > 0)
            {
                //busca en la tabla preguntas las preguntas que pertenecen al quiz
                var preguntas = context.Preguntas
                    .Include(x => x.Quiz)
                    .Include(x => x.PalabrasClave)
                    .Where(x => x.Quiz.QuizId == temaquizdto.Quiz.QuizId).ToList();

                //recorre la lista de preguntas para asignarle el valor de false y editarlo
                foreach (var item in preguntas)
                {
                    item.Activo = false;
                    var oldPregunta = await context.Preguntas.FindAsync(item.PreguntaId);
                    context.Entry(oldPregunta).CurrentValues.SetValues(item);
                    await context.SaveChangesAsync();

                    foreach (var item2 in item.PalabrasClave)
                    {
                        item2.Activo = false;
                        var oldPalabra = await context.PalabrasClave.FindAsync(item2.PalabrasClaveId);
                        context.Entry(oldPalabra).CurrentValues.SetValues(item2);
                        await context.SaveChangesAsync();
                    }
                }
            }

            //obtener el registro original usando el método FindAsync 
            var oldQuiz = await context.Quizzes.FindAsync(temaquizdto.Quiz.QuizId);

            //las propiedades sin cambios se ignoran y solo los valores de cambios se incluyen en la consulta de actualización
            context.Entry(oldQuiz).CurrentValues.SetValues(temaquizdto.Quiz);

            await context.SaveChangesAsync(user.Id);
            return NoContent();
        }

        [Route("GuardarRespuesta")]
        [HttpPost]
        public async Task<ActionResult<int>> Post(List<Respuesta> listarespuestas)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            #region GuardarRespuestas

            int Id = 0;//se tuvo que poner una variable id global para poder retonar algo ya que el return de respuesta esta adentro del if y no tiene alcance fuera de el            

            List<Pregunta> listadodepreguntas = new List<Pregunta>();
            Quiz quiz = new Quiz();

            //Se recorre la lista de respuestas para obtener cada una de las preguntas y asi obtener las respuestas originales y sus palabras claves
            foreach (var respuesta in listarespuestas)
            {
                respuesta.Activo = true;
                respuesta.FechaRegistro = DateTime.Now;

                //por cada respuestas una pregunta
                var respuestaenpregunta = await context.Preguntas
                    .Where(x => x.PreguntaId == respuesta.PreguntaId)
                    .Include(x=>x.Quiz)
                    .Include(x=>x.PalabrasClave)
                    .FirstOrDefaultAsync();

                listadodepreguntas.Add(respuestaenpregunta);

                //se recorre las preguntas para obtener las palabras clave
                foreach(var pregunta in listadodepreguntas)
                {
                    //se necesita el id del quiz para guardarlo en quizstatus
                    quiz = await context.Quizzes.Where(x => x.QuizId == pregunta.Quiz.QuizId).FirstOrDefaultAsync();

                    //se recorren las palabras clave para irlas comparando con las respuestes y evaluar si la respuesta contiene una keyword
                    foreach (var keyword in pregunta.PalabrasClave)
                    {
                        //con esto quitamos los tiles
                        /*
                         * 1-NormalizationForm.FormD,Asegura que la cadena se expanda a separa los caracterés como tides y otros modificadores en sus caracteres consitutyentes
                         * 2-y lo reemplazamos por una cadena vacía.Regex reg = new Regex("[^a-zA-Z0-9]");
                        */
                        string keywordsintildes = Regex.Replace(keyword.Palabra.Normalize(NormalizationForm.FormD), @"[^a-zA-z0-9 ]+", "");
                        string respuestasintildes = Regex.Replace(respuesta.NombreRespuesta.Normalize(NormalizationForm.FormD), @"[^a-zA-z0-9 ]+", "");

                        //con esto igrnora si son maysuculas o minusculas usamos un metodo de extension
                        bool silacontiene = respuestasintildes.Contains(keywordsintildes, StringComparison.OrdinalIgnoreCase);

                        if (silacontiene)
                        {
                            respuesta.Correcta = true;
                        }
                    }                  
                }
                context.Add(respuesta);
                await context.SaveChangesAsync(user.Id);
                Id = respuesta.RespuestaId;              
            }

            #endregion

            #region GuardarEstadoyActividadQuiz

            EstadosdelQuiz estado = new EstadosdelQuiz();
            estado.EstadoQuiz = EstadoQuiz.Terminado;
            context.Add(estado);
            await context.SaveChangesAsync(user.Id);         

            //ver si en la bd ya existe una actividad con el temaid y fasecursoid pasado por parametro
            var siexisteactividad = await context.ActividadUsuarioQuiz.AnyAsync(x => x.QuizId == quiz.TemaId && x.EstadosdelQuizId == estado.EstadosdelQuizId && x.UserId == user.Id);

            //sino exite una actividad con esos id, agregala 
            if (!siexisteactividad)
            {
                ActividadUsuarioQuiz actividad = new ActividadUsuarioQuiz();
                actividad.UserId = user.Id;
                actividad.QuizId = quiz.QuizId;
                actividad.EstadosdelQuizId = estado.EstadosdelQuizId;

                var empleado = await context.Empleados.Where(x => x.Noemp == user.noemp).FirstOrDefaultAsync();
                actividad.Idempleado = empleado.Idempleado;

                context.Add(actividad);
                await context.SaveChangesAsync(user.Id);
            }

            #endregion          

            return Id;
        }
        public static bool Contains(string textoParametro, string textoAComparar, StringComparison comparacion)
        {
            return textoParametro?.IndexOf(textoAComparar, comparacion) >= 0;
        }

        [Route("ObtenerResultados/{id}")]
        [HttpGet]
        public async Task<ActionResult<ActividadStatusQuizDTO>> GetResultados(int id)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            //se busca un quiz por que se necesita para actualizar la fase de termino de tema y el quiz contiene el id del tema
            var quiz = await context.Quizzes.Where(x => x.QuizId == id).FirstOrDefaultAsync();

            #region ObtenerEstadoQuiz 

            //buscamos una actividad por medio del id del quiz enviado por parametro
            var actividad = await context.ActividadUsuarioQuiz
                .Where(x => x.UserId == user.Id && x.QuizId == id).FirstOrDefaultAsync();

            var model = new ActividadStatusQuizDTO();

            //si la actividad es diferente de null se hace todos los procesos siguientes
            if (actividad != null)
            {
                //en base a la actividad buscamos un estado del quiz 
                var estado = await context.EstadosdelQuiz
                   .Where(x => x.EstadosdelQuizId == actividad.EstadosdelQuizId).FirstOrDefaultAsync();

                //la actividad y el estado del quiz se adjunto a un nuevo dto que sera lo que retornaremos
                model.ActividadUsuarioQuiz = actividad;
                model.EstadosdelQuiz = estado;

                #region ObtenerCalificacion

                //tenemos que buscar todas las preguntas que pertenezcan al quiz que esta siendo evaluado
                var listadodepreguntas = await context.Preguntas.Where(x => x.Quiz.QuizId == id).Include(x => x.Quiz).ToListAsync();
                List<Respuesta> listadoderespuestas = new List<Respuesta>();

                //recorremos el listado de preguntas para obtener un listado de respuestas
                foreach (var pregunta in listadodepreguntas)
                {
                    var respuesta = await context.Respuestas.Where(x => x.PreguntaId == pregunta.PreguntaId && x.NombreUsuario == user.noemp).FirstOrDefaultAsync();

                    //si la respuesta es diferente de null se agrega al listado de respuestas
                    if(respuesta != null)
                    {
                        listadoderespuestas.Add(respuesta);
                    }                  
                }

                int totaldepreguntas = 0;
                int respuestascorrectas = 0;
                int porcentaje = 0;

                //si el listado de respuestas es diferente de null y mayor a 0, obtenemos las evaluaciones
                if (listadoderespuestas.Count > 0)
                {
                    totaldepreguntas = listadodepreguntas.Count();
                    respuestascorrectas = listadoderespuestas.Where(x => x.Correcta == true).Count();
                    porcentaje = (int)Math.Round((double)(100 * respuestascorrectas) / totaldepreguntas);                   
                }

                //adjuntamos las evaluaciones al dto que vamos a retornar
                model.TotaldePreguntas = totaldepreguntas;
                model.TotalRespuestasCorrectas = respuestascorrectas;
                model.Porcentaje = porcentaje;

                //si en el quiz hay 3 respuestas correctas se evalua como aprobado sino como reprobado
                if(respuestascorrectas >= 3)
                {
                    estado.Evaluacion = Evaluacion.Aprobado;

                    #region TerminarTema
                    /*
                    1-Si es aprobado se actualiza el estado del tema y la fase 3 se da por terminada
                    2-Sino aprueba el tema no sera finalizado
                    3-Sino finaliza el tema jamas avanzara para obtener el certificado
                    4-Ya que la logica de fin de curso necesita las 3 fases del tema terminadas para obtener el certificado
                    */

                    //cuando el usuario vaya a ver sus resultados del quiz se actualizara la fase termino del tema                

                    ActividadUsuario ActividadUsuario = new ActividadUsuario();
                    ActividadUsuario.UserId = user.Id;
                    ActividadUsuario.IsComplete = true;
                    ActividadUsuario.TemaId = quiz.TemaId;
                    ActividadUsuario.FaseCursoId = 3;

                    //ver si en la bd ya existe una actividad con el temaid, fasecuroid y userid
                    var siexisteactividad = await context.ActividadUsuarios.AnyAsync(x => x.TemaId == ActividadUsuario.TemaId && x.FaseCursoId == ActividadUsuario.FaseCursoId && x.UserId == user.Id);

                    //sino exite, agregala
                    if (!siexisteactividad)
                    {
                        var empleado = await context.Empleados.Where(x => x.Noemp == user.noemp).FirstOrDefaultAsync();
                        ActividadUsuario.Idempleado = empleado.Idempleado;

                        context.Add(ActividadUsuario);
                        await context.SaveChangesAsync(user.Id);
                    }              

                    #endregion
                }
                else
                {
                    estado.Evaluacion = Evaluacion.Reprobado;                 
                }

                //con el estado que obtuvimos actualizamos el estado de la bd solo con la diferencia que ahora tendra la evaluacion aprobado o rerpobado
                var oldestado = await context.EstadosdelQuiz.FindAsync(estado.EstadosdelQuizId);
                context.Entry(oldestado).CurrentValues.SetValues(estado);
                await context.SaveChangesAsync(user.Id);

                #endregion
            }

            #endregion

            return model;
        }

        [Route("ListadeActividadesTerminadas/{id}")]
        [HttpGet]
        public async Task<ActionResult<ActividadesFasesTerminadasDTO>> GetFasesTerminadas(int id)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            //buscamos si existe un registro en la tabla cursoestado que sea del curso envidado por parametro y con status terminado
            var Existeestadocurso = await context.CursoEstado.AnyAsync(x => x.CursoId == id && x.EstadoCurso == EstadoCurso.Terminado && x.UserId == user.Id);

            //buscamos un listado de temas por el id del curso enviado
            var listadetemas = await context.Temas.Where(x => x.CursoId == id).ToListAsync();

            List<ActividadUsuario> listadeactividades = new List<ActividadUsuario>();
            bool FasesTerminadas = false;

            //recorremos el listado de temas para obtener una actividad por tema, filtrado por curso seleccionado
            foreach (var tema in listadetemas)
            {
                //sino existe un un registro en la tabla curso estado con estatus terminado, añade actividades a la lista
                if (!Existeestadocurso)
                {
                    //si la actividad tiene la fase 3 completada y es del mismo tema y del mismo usuario entonces agregala a la lista
                    var actividad = await context.ActividadUsuarios
                       .Where(x => x.IsComplete == true && x.UserId == user.Id && x.FaseCursoId == 3 && x.TemaId == tema.TemaId)
                       .FirstOrDefaultAsync();

                    //evita añadir actividades nulas a la lista de actividades
                    if (actividad != null)
                    {
                        listadeactividades.Add(actividad);
                    }
                }
                else
                {
                    //pero si ya hay actividades con fase 4 terminada mandamos una bandera de terminado
                    FasesTerminadas = true;
                }                            
            }        

            //se uso un dto solo por el hecho de mandar una bandera de termino de fases al lado del cliente, "se podria usar otra forma de avisar que se termino el tema y el curso"
            var model = new ActividadesFasesTerminadasDTO();
            model.ListadeActiviades = listadeactividades;
            model.Terminado = FasesTerminadas;

            //retornamos una lista de actividades
            return model;           
        }
       
        [Route("TerminarFases/{id}")]
        [HttpPost]
        public async Task<ActionResult<int>> Post(ActividadUsuario actividad, int id)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            //buscamos un listado de temas por el id del curso enviado
            var listadetemas = await context.Temas.Where(x => x.CursoId == id).ToListAsync();

            List<ActividadUsuario> listadeactividades = new List<ActividadUsuario>();

            //recorremos el listado de temas para obtener una actividad por tema, filtrado por curso seleccionado
            foreach (var tema in listadetemas)
            {
                //si la actividad tiene la fase 3 completada y es del mismo tema y del mismo usuario entonces agregala a la lista
                var actividadIn = await context.ActividadUsuarios
                   .Where(x => x.IsComplete == true && x.UserId == user.Id && x.FaseCursoId == 3 && x.TemaId == tema.TemaId)
                   .FirstOrDefaultAsync();

                //evita añadir actividades nulas a la lista de actividades
                if (actividadIn != null)
                {
                    listadeactividades.Add(actividadIn);
                }
            }

            List<ActividadUsuario> listadefasesterminadas = new List<ActividadUsuario>();

            //recorremos la lista de actividades, por cada iteracion se va a crear una nueva actividad 
            foreach (var item in listadeactividades)
            {
                ActividadUsuario actividadUsuario = new ActividadUsuario();
                actividadUsuario.IsComplete = actividad.IsComplete;
                actividadUsuario.FaseCursoId = actividad.FaseCursoId;
                actividadUsuario.UserId = user.Id;
                actividadUsuario.TemaId = item.TemaId;

                //ver si en la bd ya existe una actividad con el temaid y fasecursoid pasado por parametro
                var siexisteactividad = await context.ActividadUsuarios
                .AnyAsync(x => x.TemaId == actividadUsuario.TemaId && x.FaseCursoId == actividadUsuario.FaseCursoId && x.UserId == actividadUsuario.UserId);

                //sino exite una actividad con esos id, agregala a la lista
                if (!siexisteactividad)
                {
                    var empleado = await context.Empleados.Where(x => x.Noemp == user.noemp).FirstOrDefaultAsync();
                    actividadUsuario.Idempleado = empleado.Idempleado;

                    listadefasesterminadas.Add(actividadUsuario);
                }               
            }

            /*1-para no ir guardando por iteracion las actividades dentro del foreach
            2-se guardaron las actividades en una lista 
            3-enviaremos a la bd toda la data junta de dicha lista usando addrange*/
            context.AddRange(listadefasesterminadas);
            await context.SaveChangesAsync(user.Id);

            //si la lista es iguala 5 entonces quiere decir que todas las fases estan terminadas y el curso ya finalizo, entonces guarda el estado del curso
            if(listadefasesterminadas.Count == 3)
            {
                CursoEstado cursoEstado = new CursoEstado();
                cursoEstado.CursoId = id;
                cursoEstado.UserId = user.Id;
                cursoEstado.EstadoCurso = EstadoCurso.Terminado;

                context.CursoEstado.Add(cursoEstado);
                await context.SaveChangesAsync(user.Id);
            }
            
            return actividad.ActividadUsuarioId;
        }       

        //utilizamos fromquery para traer los parametros de busqueda
        [HttpGet("filtrar")]
        [AllowAnonymous]
        public async Task<ActionResult<List<ActividadUsuario>>> Get([FromQuery] ParametrosBusqueda parametrosBusqueda)
        {
            List<ActividadUsuario> listadeactividades =
                (from e in context.ActividadUsuarios
                 select new ActividadUsuario
                 {
                     ActividadUsuarioId = e.ActividadUsuarioId,
                     UserId = e.UserId,
                     IsComplete = e.IsComplete,
                     FaseCursoId = e.FaseCursoId,
                     Idempleado = e.Idempleado,
                     Tema = context.Temas.Where(x => x.TemaId == e.TemaId).FirstOrDefault(),
                     FaseCurso = context.FaseCursos.Where(x => x.FaseCursoId == e.FaseCursoId).FirstOrDefault(),
                     Empleado = (from z in context.Empleados
                                 where z.Idempleado == e.Idempleado
                                 select new Empleado
                                 {
                                     Noemp = z.Noemp,
                                     Persona = context.Personas.Where(x => x.Idpersona == z.Idpersona).FirstOrDefault(),

                                 }).FirstOrDefault()
                 }).ToList();

            var queryable = listadeactividades.OrderBy(x => x.Empleado.Noemp).ThenBy(x=>x.TemaId).AsQueryable();

            if (parametrosBusqueda.EmpleadoId != 0)
            {
                queryable = queryable.Where(x => x.Idempleado == parametrosBusqueda.EmpleadoId);
            }           

            //paginacion
            await HttpContext.InsertarParametrosPaginacionEnRespuesta(queryable, parametrosBusqueda.CantidadRegistros, true);
            var listaARetornar = queryable.Paginar(parametrosBusqueda.Paginacion).ToList();
            return listaARetornar;
        }
        public class ParametrosBusqueda
        {
            public int Pagina { get; set; } = 1;
            public int CantidadRegistros { get; set; } = 12;
            public PaginacionDTO Paginacion
            {
                get { return new PaginacionDTO() { Pagina = Pagina, CantidadRegistros = CantidadRegistros }; }
            }
            public int EmpleadoId { get; set; }         
        }
    }
}
