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
        public async Task<ActionResult<int>> Post(TemaQuizDTO temaquizdto)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            //guardamos el tema
            temaquizdto.Tema.Activo = true;

            if (string.IsNullOrEmpty(temaquizdto.Tema.Nombre))
            {
                string mensajeError = "Debe ingresar un nombre para el tema";
                return BadRequest(mensajeError);
            }

            if (string.IsNullOrEmpty(temaquizdto.Tema.Descripcion))
            {
                string mensajeError = "Debe ingresar una descripción para el tema";
                return BadRequest(mensajeError);
            }

            context.Add(temaquizdto.Tema);
            await context.SaveChangesAsync(user.Id);

            //traemos un listado de fases
            var listadodefases = await context.FaseCursos.ToListAsync();

            //recorremos el listado de fases y por cada fase se va a crear un objeto de tipo TemaFase, con el fin de que cada tema tenga varias fases
            foreach(var item in listadodefases)
            {
                TemaFase temaFase = new TemaFase();
                temaFase.TemaId = temaquizdto.Tema.TemaId;
                temaFase.FaseCursoId = item.FaseCursoId;

                temaFase.Tema = null;
                temaFase.FaseCurso = null;

                context.Add(temaFase);
                await context.SaveChangesAsync(user.Id);
            }

            //guardamos el quiz
            //si el quiz trae por lo menos un nombre quiere decir que si querian crear un quiz, sin el if se crearia al actualizar el tema por que no seria null
            if (!string.IsNullOrEmpty(temaquizdto.Quiz.Nombre))
            {
                temaquizdto.Quiz.TemaId = temaquizdto.Tema.TemaId;
                temaquizdto.Quiz.Activo = true;

                //recorremos una lista de preguntas, a cada pregunta le ponemos un valor de activo y su fecha de registro
                foreach (var item in temaquizdto.Quiz.LisadePreguntas)
                {
                    item.FechaRegistro = DateTime.Now;
                    item.Activo = true;

                    //si se intenta guardar un quiz sin una pregunta lanza un error de que debe de añadir una
                    if (string.IsNullOrEmpty(item.NombrePregunta))
                    {
                        string mensajeError = "Ingresa una pregunta";
                        return BadRequest(mensajeError);
                    }

                    //recorremos una lista de palabras clave por cada pregunta
                    foreach(var item2 in item.PalabrasClave)
                    {
                        item2.FechadeRegistro = DateTime.Now;
                        item2.Activo = true;

                        //si se intenta guardar una pregunta sin una palabra clave lanza un error de que debe de añadir una
                        if (string.IsNullOrEmpty(item2.Palabra))
                        {
                            string mensajeError = "Ingresa por lo menos una palabra clave";
                            return BadRequest(mensajeError);
                        }
                    }

                }
                context.Add(temaquizdto.Quiz);
            }  
            
            await context.SaveChangesAsync(user.Id);
            return temaquizdto.Tema.TemaId;
        }
       
        [HttpGet("{id}")]
        public async Task<ActionResult<TemaQuizDTO>> Get(int id)
        {
            //obtenemos un tema
            var tema = await context.Temas.Where(x => x.TemaId == id).FirstOrDefaultAsync();
            if (tema == null) { return NotFound(); }

            //si el usuario no subio imagen poner una por defecto
            if (string.IsNullOrEmpty(tema.Imagen))
            {
                // Aquí colocas la URL de la imagen por defecto
                tema.Imagen = "Img" + "/" + "Imagenotfound.jpg";
            }

            /*1 - La logica de eliminar cuestionario esta bien.
            2 - si se elimina un cuestionario desde la "razor detalles tema", se debe dejar de ver ese cuestionario.
            3 - En editar tema, ya no debe de estar disponible ese cuestionario ni siquiera para editarlo y volverlo activar.
            4 - La  relacion es de uno a uno, aunque se vuelva activar el cuestionario eliminado, las demas configuracion solo
                permitira visualizar un cuestionario, no importando que el tema tenga mas cuestionarios activos y ligados a el.
            5 - Se debe crear un nuevo cuestionario para ese tema no revivir el cuestionario muerto.
            */
              //obtenemos un quiz por tema
              var quiz = await context.Quizzes.Where(x => x.TemaId == id && x.Activo == true)
              .Include(x => x.LisadePreguntas).ThenInclude(x=>x.PalabrasClave)              
              .FirstOrDefaultAsync();

            //si el quiz es diferente de null y no trae imagen le añadimos una por defecto
            if(quiz != null)
            {                
                if (string.IsNullOrEmpty(quiz.Imagen))
                {
                    // Aquí colocas la URL de la imagen por defecto
                    quiz.Imagen = "Img" + "/" + "Imagenotfound.jpg";
                }
            } 

            //retornamos un dto con ambos objetos
            var model = new TemaQuizDTO();
            model.Quiz = quiz;
            model.Tema = tema;           
            return model;
        }
        
        [HttpPut]
        public async Task<ActionResult> Put(TemaQuizDTO temaquizDTO)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            #region EditarTema
            //buscamos un tema en la bd por el id del tema
            var TemaDB = await context.Temas.FirstOrDefaultAsync(x => x.TemaId == temaquizDTO.Tema.TemaId);
            if (TemaDB == null) { return NotFound(); }
            //si el tema de la bd no tiene imagen le ponemos la nueva que viene por parametro dentro del objeto
            if (!string.IsNullOrWhiteSpace(temaquizDTO.Tema.Imagen))
            {
                TemaDB.Imagen = temaquizDTO.Tema.Imagen;
            }
            //mapeamos el tema de la bd con el que viene como paramtro
            TemaDB = mapper.Map(temaquizDTO.Tema, TemaDB);
            await context.SaveChangesAsync(user.Id);
            #endregion

            #region EditarCuandoseBorraTema
            //si un tema se borra y se vuelve a activar se debe activar con su cuestionario que tenia                      

            if (temaquizDTO.Quiz.Activo == false)
            {
                //como el dto el objecto quiz viene vacio se tiene que buscar el quiz relacionado por tema,
                //este quiz cumplira la funcion como el quiz enviado como parametro
                var QuizDBEliminada = await context.Quizzes
                    .Where(x => x.TemaId == temaquizDTO.Tema.TemaId)
                    .Include(x => x.LisadePreguntas).ThenInclude(x=>x.PalabrasClave)
                    .FirstOrDefaultAsync();

                if(QuizDBEliminada != null)
                {
                    //se tiene que volver a buscar el mismo quiz, que buscamos arriba pero esta vez para editarlo
                    var oldQuiz = await context.Quizzes.FindAsync(QuizDBEliminada.QuizId);

                    //actualizamos las preguntas
                    foreach (var pregunta in QuizDBEliminada.LisadePreguntas)
                    {
                        //si la pregunta el id es diferente de 0 quiere decir que se modificara
                        if (pregunta.PreguntaId != 0)
                        {
                            //como se modificara se agrega el true para volverlo activar
                            pregunta.Activo = true;
                            context.Entry(pregunta).State = EntityState.Modified;
                        }

                        foreach (var keyword in pregunta.PalabrasClave)
                        {
                            //si la palabra clave el id es diferente de 0 se modificara sino se añadira
                            if (keyword.PalabrasClaveId != 0)
                            {
                                keyword.Activo = true;
                                context.Entry(keyword).State = EntityState.Modified;
                            }
                        }
                    }

                    //si el tema nuevamente esta activo el quiz tambien debe estar activo
                    if (temaquizDTO.Tema.Activo == true)
                    {
                        QuizDBEliminada.Activo = true;
                    }

                    //actualizamos el estado del quiz a activo
                    context.Entry(oldQuiz).CurrentValues.SetValues(QuizDBEliminada);
                    await context.SaveChangesAsync(user.Id);
                }             
            }

            #endregion

            #region EditarQuiz
            //si la data del quiz no trae id del quiz quiere decir que se va a crear un nuevo quiz
            if (temaquizDTO.Quiz.QuizId == 0)
            {
                //si el quiz no trae nombre reafirma que se quiere crear un quiz y no editarlo
                if (!string.IsNullOrEmpty(temaquizDTO.Quiz.Nombre))
                {
                    //se repiten los pasos que se realizan en la api post del tema
                    temaquizDTO.Quiz.TemaId = temaquizDTO.Tema.TemaId;
                    temaquizDTO.Quiz.FechaRegistro = DateTime.Now;
                    temaquizDTO.Quiz.Activo = true;

                    foreach (var item in temaquizDTO.Quiz.LisadePreguntas)
                    {
                        item.FechaRegistro = DateTime.Now;
                        item.Activo = true;

                        if (string.IsNullOrEmpty(item.NombrePregunta))
                        {
                            string mensajeError = "Ingresa una pregunta";
                            return BadRequest(mensajeError);
                        }

                        foreach (var item2 in item.PalabrasClave)
                        {
                            item2.FechadeRegistro = DateTime.Now;
                            item2.Activo = true;

                            if (string.IsNullOrEmpty(item2.Palabra))
                            {
                                string mensajeError = "Ingresa por lo menos una palabra clave";
                                return BadRequest(mensajeError);
                            }
                        }

                    }
                    context.Add(temaquizDTO.Quiz);
                    await context.SaveChangesAsync(user.Id);
                }               
            }
            else
            {
                //si la data del quiz ya trae un id quiere decir que se desea editar

                var QuizDB = await context.Quizzes.FirstOrDefaultAsync(x => x.QuizId == temaquizDTO.Quiz.QuizId);
                if (QuizDB == null) { return NotFound(); }

                //evitamos el mapeo automatico y lo hacemos de manera manual para no estar actualizando la foto sino es necesario
                if (!string.IsNullOrWhiteSpace(temaquizDTO.Quiz.Imagen))
                {
                    QuizDB.Imagen = temaquizDTO.Quiz.Imagen;
                }

                //actualizamos las preguntas
                foreach (var pregunta in temaquizDTO.Quiz.LisadePreguntas)
                {
                    //si la pregunta el id es diferente de 0 quiere decir que se modificara
                    if (pregunta.PreguntaId != 0)
                    {
                        context.Entry(pregunta).State = EntityState.Modified;
                    }
                    else
                    {
                        //sino la pregunta se añadira
                        if (string.IsNullOrEmpty(pregunta.NombrePregunta))
                        {
                            string mensajeError = "Ingresa una pregunta";
                            return BadRequest(mensajeError);
                        }
                        pregunta.FechaRegistro = DateTime.Now;
                        pregunta.Activo = true;
                        context.Entry(pregunta).State = EntityState.Added;
                    }

                    //buscar las respuestas que pertenecen a las preguntas
                    //var respuestas = context.Respuestas.Where(x => x.PreguntaId == pregunta.PreguntaId).ToList();

                    ////recorre la lista de respuestas para asignarle el valor de false y editarlo
                    //foreach (var item2 in respuestas)
                    //{
                    //    item2.Activo = true;
                    //    var oldRespuesta = await context.Respuestas.FindAsync(item2.RespuestaId);
                    //    context.Entry(oldRespuesta).CurrentValues.SetValues(item2);
                    //    await context.SaveChangesAsync();
                    //}

                    foreach (var keyword in pregunta.PalabrasClave)
                    {
                        //si la palabra clave el id es diferente de 0 se modificara sino se añadira
                        if (keyword.PalabrasClaveId != 0)
                        {
                            context.Entry(keyword).State = EntityState.Modified;
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(keyword.Palabra))
                            {
                                string mensajeError = "Ingresa por lo menos una palabra clave";
                                return BadRequest(mensajeError);
                            }

                            keyword.FechadeRegistro = DateTime.Now;
                            keyword.Activo = true;
                            context.Entry(keyword).State = EntityState.Added;
                        }
                    }
                }

                //mapeamos el quiz de la bd con el que viene como parametro               
                QuizDB = mapper.Map(temaquizDTO.Quiz, QuizDB);
                await context.SaveChangesAsync(user.Id);               
            }
            #endregion

            return NoContent();
        }

        //editar los id de video y pdf relacionados al tema
        [Route("Desactivar")]
        [HttpPut]
        public async Task<ActionResult> PutDesactivar(TemaQuizDTO temaquizdto)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            //obtener el registro original usando el método FindAsync 
            var oldtema = await context.Temas.FindAsync(temaquizdto.Tema.TemaId);

            //las propiedades sin cambios se ignoran y solo los valores de cambios se incluyen en la consulta de actualización
            context.Entry(oldtema).CurrentValues.SetValues(temaquizdto.Tema);

            await context.SaveChangesAsync(user.Id);
            return NoContent();
        }

        //desactivar el tema y el quiz al mismo tiempo si se elimina un tema
        [Route("DesactivarTemaQuiz")]
        [HttpPut]
        public async Task<ActionResult> PutDesactivar(Tema tema)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);            
            var oldtema = await context.Temas.FindAsync(tema.TemaId);           
            context.Entry(oldtema).CurrentValues.SetValues(tema);
            await context.SaveChangesAsync(user.Id);

            var quiz = await context.Quizzes.Where(x => x.TemaId == tema.TemaId)
              .Include(x => x.LisadePreguntas).FirstOrDefaultAsync();

            quiz.Activo = false;

            //si hay una pregunta entra al if para editar el campo activo
            if (quiz.LisadePreguntas.Count > 0)
            {
                //busca en la tabla preguntas las preguntas que pertenecen al quiz
                var preguntas = context.Preguntas
                    .Include(x => x.Quiz) 
                    .Include(x => x.PalabrasClave)
                    .Where(x => x.Quiz.QuizId == quiz.QuizId).ToList();

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
            var oldQuiz = await context.Quizzes.FindAsync(quiz.QuizId);

            //las propiedades sin cambios se ignoran y solo los valores de cambios se incluyen en la consulta de actualización
            context.Entry(oldQuiz).CurrentValues.SetValues(quiz);

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
