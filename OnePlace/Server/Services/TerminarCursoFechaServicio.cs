using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnePlace.Server.Data;
using OnePlace.Shared.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static OnePlace.Server.Data.ApplicationUser;

namespace OnePlace.Server.Services
{
    public interface ITerminarCursoFechaServicio
    {
        Task TerminarCursoporFecha();        
    }
    public class TerminarCursoFechaServicio : ITerminarCursoFechaServicio
    {
        private readonly oneplaceContext context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ILogger<TerminarCursoFechaServicio> logger;       
        public TerminarCursoFechaServicio(oneplaceContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor, ILogger<TerminarCursoFechaServicio> logger)
        {
            this.context = context;
            this.logger = logger;            
            _userManager = userManager;
            this.httpContextAccessor = httpContextAccessor;
        }

        //public string UserId { get { return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier).Value; } }
        public async Task TerminarCursoporFecha()
        {
            logger.LogInformation("Se ejecuto un job para terminar los cursos por fecha");

            //var user = await _userManager.GetUserAsync(httpContextAccessor.HttpContext.User);          

            #region LlenarBDparaTerminarFases

            //obtenemos la fecha de hoy
            var fechadehoy = DateTime.Today;

            //listado de cursos que ya vencieron y sus respectivos temas
            var curso = await context.Cursos.Where(x => x.FechaFinal <= fechadehoy)
               .Include(x => x.LisadeTemas).FirstOrDefaultAsync();

            //listado de usuarios para obtener iduser y idempleado
            var users = await context.Users.Where(x => x.Activo == true && x.TipodeUsuarios == TipodeUsuario.Usuario.ToString()).ToListAsync();

            //listados de actividades
            List<ActividadUsuario> listadeactividadesARetornar = new List<ActividadUsuario>();
            List<ActividadUsuario> listadeactividades = new List<ActividadUsuario>();

            //recorremos el listado de empleado contra la lista de temas
            foreach (var item in users)
            {
                foreach (var tema in curso.LisadeTemas)
                {
                    //si existe una activiadad con estos filtros:
                    //-UserId igual al userid de la lista de usuarios
                    //-Status IsComplete en true
                    //-TemaId igual al id del listado de temas 
                    //FaseId igual a la fase 1
                    //no se crea dicha actividad en caso contrario entra a un a evaluar una segunda condicion
                    var actividadusuariofase1true = await context.ActividadUsuarios
                       .AnyAsync(x => x.UserId == item.Id && x.IsComplete == true && x.TemaId == tema.TemaId && x.FaseCursoId == 1);

                    if (!actividadusuariofase1true)
                    {
                        //se repite el mismo filtro de busqueda de actividad a diferencia que aqui buscamos las actividades con status iscomplete false
                        var actividadusuariofase1false = await context.ActividadUsuarios
                       .AnyAsync(x => x.UserId == item.Id && x.IsComplete == false && x.TemaId == tema.TemaId && x.FaseCursoId == 1);

                        if (!actividadusuariofase1false)
                        {
                            //si en la bd no existe ningunga actividad con los criterios antes evaluados ahora si pasamos a crear dicha actividad en la bd
                            ActividadUsuario actividadUsuario = new ActividadUsuario();
                            actividadUsuario.UserId = item.Id;
                            actividadUsuario.IsComplete = false;
                            actividadUsuario.TemaId = tema.TemaId;
                            actividadUsuario.FaseCursoId = 1;
                            actividadUsuario.Idempleado = item.Idempleado;

                            context.Add(actividadUsuario);
                            await context.SaveChangesAsync();

                        }
                    }

                    //se repite los pasos que describimos arriba.
                    var actividadusuariofase2true = await context.ActividadUsuarios
                       .AnyAsync(x => x.UserId == item.Id && x.IsComplete == true && x.TemaId == tema.TemaId && x.FaseCursoId == 2);

                    if (!actividadusuariofase2true)
                    {
                        var actividadusuariofase2false = await context.ActividadUsuarios
                       .AnyAsync(x => x.UserId == item.Id && x.IsComplete == false && x.TemaId == tema.TemaId && x.FaseCursoId == 2);

                        if (!actividadusuariofase2false)
                        {
                            ActividadUsuario actividadUsuario = new ActividadUsuario();
                            actividadUsuario.UserId = item.Id;
                            actividadUsuario.IsComplete = false;
                            actividadUsuario.TemaId = tema.TemaId;
                            actividadUsuario.FaseCursoId = 2;
                            actividadUsuario.Idempleado = item.Idempleado;

                            context.Add(actividadUsuario);
                            await context.SaveChangesAsync();

                        }
                    }

                    var actividadusuariofase3true = await context.ActividadUsuarios
                      .AnyAsync(x => x.UserId == item.Id && x.IsComplete == true && x.TemaId == tema.TemaId && x.FaseCursoId == 3);

                    if (!actividadusuariofase3true)
                    {
                        var actividadusuariofase3false = await context.ActividadUsuarios
                       .AnyAsync(x => x.UserId == item.Id && x.IsComplete == false && x.TemaId == tema.TemaId && x.FaseCursoId == 3);

                        if (!actividadusuariofase3false)
                        {
                            ActividadUsuario actividadUsuario = new ActividadUsuario();
                            actividadUsuario.UserId = item.Id;
                            actividadUsuario.IsComplete = false;
                            actividadUsuario.TemaId = tema.TemaId;
                            actividadUsuario.FaseCursoId = 3;
                            actividadUsuario.Idempleado = item.Idempleado;

                            context.Add(actividadUsuario);
                            await context.SaveChangesAsync();

                        }
                    }

                    //si la actividad tiene la fase 3 pero su estado es incompleto y es del mismo tema y del mismo usuario entonces agregala a la lista
                    var actividadIn = await context.ActividadUsuarios
                       .Where(x => x.UserId == item.Id && x.IsComplete == false && x.TemaId == tema.TemaId && x.FaseCursoId == 3)
                       .FirstOrDefaultAsync();

                    //evita añadir actividades nulas a la lista de actividades
                    if (actividadIn != null)
                    {
                        listadeactividades.Add(actividadIn);
                    }
                }

                /*aqui comparamos las listas para poder obtener los resultados esperados
                si solo se recorre las actividades dentro del foreach temas solo obtendremos las actividades del primer usuario
                y no las actividades de todos los usuarios, aqui vamos llenando un nuevo listado en el foreach de los usuarios por cada usuario*/
                listadeactividadesARetornar = listadeactividades;

                //cambiar a 4(4 temas finalizados)
                /*
                Si la lista es igual a 3, quiere decir que reporbo 3 temas
                (termino el video, termino el quiz pero no lo aprobo, y por lo tanto la fase 3 se pone como terminada pero con estado incompleto)
                Si la lista es menor a 3 quiere decir que de los 3 temas uno o dos no los termino satisfactoriamente.
                Ademas se pone un filtro por id usuario para que contabilice por separado las actividades de cada usuario, sino solo cuenta las de uno
                */
                if (listadeactividadesARetornar.Where(x => x.UserId == item.Id).Count() == 3 || listadeactividadesARetornar.Where(x => x.UserId == item.Id).Count() <= 3)
                {
                    //buscamos si en la tabla estadocurso no exite ya un registro, no importa si su estado es completado o incompleto 
                    var existecursoestado = await context.CursoEstado
                        .AnyAsync(x => x.CursoId == curso.CursoId && x.UserId == item.Id);

                    //sino exite, añadimos un nuevo registro de estadocurso con el estado sin completar
                    if (!existecursoestado)
                    {
                        CursoEstado cursoEstado = new CursoEstado();
                        cursoEstado.CursoId = curso.CursoId;
                        cursoEstado.Idempleado = item.Idempleado;
                        cursoEstado.UserId = item.Id;
                        cursoEstado.EstadoCurso = EstadoCurso.SinCompletar;

                        context.CursoEstado.Add(cursoEstado);
                        await context.SaveChangesAsync();
                    }
                }
            }

            #endregion

            //return true;
        }
    }
}
