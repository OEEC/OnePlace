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
    public class ActividadUsuarioQuizController : ControllerBase
    {

        private readonly oneplaceContext context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper mapper;
        public ActividadUsuarioQuizController(oneplaceContext context, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            this.context = context;
            _userManager = userManager;
            this.mapper = mapper;
        }

        [HttpGet("filtrar")]
        [AllowAnonymous]
        public async Task<ActionResult<List<ActividadUsuarioQuiz>>> Get([FromQuery] ParametrosBusqueda parametrosBusqueda)
        {
            bool mostrar = true;
            //query para traer los eventos
            //var queryable = context.Eventos.Where(x => x.Activo == mostrar).OrderBy(x => x.EventoId).AsQueryable();

            /*var queryable = context.ActividadUsuarioQuiz
               .Include(x => x.Quiz)
               .Include(x => x.User)
               .AsQueryable();*/

            var queryable = (
                from auq in context.ActividadUsuarioQuiz
                join u in context.Users on auq.UserId equals u.Id
                select new ActividadUsuarioQuiz
                {
                    UserId = auq.UserId,
                    QuizId = auq.QuizId,
                    EstadosdelQuizId = auq.EstadosdelQuizId,
                    Idempleado = auq.Idempleado,
                    NombreEmpleado = u.Nombre + ' ' + u.ApellidoPaterno + ' ' + u.ApellidoMaterno,
                }

                ).AsQueryable();


            if (parametrosBusqueda.QuizId != 0)
            {
                queryable = queryable.Where(x => x.QuizId == parametrosBusqueda.QuizId);
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
            public int QuizId { get; set; }
            public bool Activo { get; set; }
        }

    }
}
