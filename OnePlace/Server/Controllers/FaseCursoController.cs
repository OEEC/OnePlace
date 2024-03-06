using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnePlace.Server.Data;
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
    public class FaseCursoController : ControllerBase
    {
        private readonly oneplaceContext context;
        private readonly UserManager<ApplicationUser> _userManager;
        public FaseCursoController(oneplaceContext context, UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            _userManager = userManager;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TemaFaseDTO>> Get(int id)
        {       
            //obtenemos el id del usuario que inicio sesion
            var user = await _userManager.GetUserAsync(HttpContext.User); 

            //buscamos un listado de temasfases que coincida con el id del tema enviado
            var listadodetemasyfases = await context.TemaFases.Where(x => x.TemaId == id).ToListAsync();

            List<FaseCurso> listadefases = new List<FaseCurso>();          
       
            //recorremos el listado de temasfases relacionados al id del tema
            foreach(var item in listadodetemasyfases)
            {
                //creamos un listado de fases, solo con las fases que pertenecen a un tema en especifico
                var fasecurso = await context.FaseCursos.Where(x => x.FaseCursoId == item.FaseCursoId).FirstOrDefaultAsync();
                listadefases.Add(fasecurso);      
            }

            //obtenemos un listado de actividades que pertenezcan al id del tema enviado y ademas que solo sean del usuario logueado
            //esto para evitar que otro usuario vea el avance de otros usuarios en su perfil
            var actividades = await context.ActividadUsuarios
                .Where(x => x.TemaId == id && x.UserId == user.Id)
                .ToListAsync();

            //buscamos un quiz con el id del tema enviado
            var quiz = await context.Quizzes.Where(x => x.TemaId == id).FirstOrDefaultAsync();

            EstadosdelQuiz estadoquiz = new EstadosdelQuiz();

            if (quiz != null)
            {
                //buscamos una actividad quiz con el id del quiz antes buscado, y que pertenezca al usuario logueado
                var actividadquiz = await context.ActividadUsuarioQuiz.Where(x => x.QuizId == quiz.QuizId && x.UserId == user.Id).FirstOrDefaultAsync();

                //si encuentra una actividad entonces busca el estado de esa actividad donde sea terminado
                if (actividadquiz != null)
                {
                    estadoquiz = await context.EstadosdelQuiz
                      .Where(x => x.EstadosdelQuizId == actividadquiz.EstadosdelQuizId && x.EstadoQuiz == EstadoQuiz.Terminado)
                      .FirstOrDefaultAsync();
                }
            }                 

            //retornamos un dto con ambas listas para ser usadas en el timeline de progreso
            var model = new TemaFaseDTO();
            model.ListadodeFases = listadefases;
            model.ListadeActividades = actividades;
            model.EstadosdelQuiz = estadoquiz;//el enviar el estado del quiz en terminado , nos ayudara a cambiar el boton de responder cuestionario a ver respuestas

            return model;
        }      

        //cuando el usuario termina una actividad dentro del curso se crea una nueva actividad en la base de datos
        [HttpPost]
        public async Task<ActionResult<int>> Post(ActividadUsuario actividad)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var empleado = await context.Empleados.Where(x => x.Noemp == user.noemp).FirstOrDefaultAsync();
            actividad.Idempleado = empleado.Idempleado;

            var fasecurso = context.FaseCursos.Where(x => x.TemaId == actividad.TemaId).FirstOrDefault();
            if(fasecurso is null) { return NotFound(); }

            actividad.FaseCursoId = fasecurso.FaseCursoId;

            //ver si en la bd ya existe una actividad con el temaid y fasecursoid pasado por parametro
            var siexisteactividad = await context.ActividadUsuarios.AnyAsync(x => x.TemaId == actividad.TemaId && x.FaseCursoId == actividad.FaseCursoId && x.UserId == actividad.UserId);

            //sino exite una actividad con esos id, agregala 
            if (!siexisteactividad)
            {                
                context.Add(actividad);
                await context.SaveChangesAsync(user.Id);
            }   
            
            return actividad.ActividadUsuarioId;
        }
    }
}
