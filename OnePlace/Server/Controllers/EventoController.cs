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
    public class EventoController : ControllerBase
    {
        private readonly oneplaceContext context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper mapper;
        public EventoController(oneplaceContext context, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            this.context = context;
            _userManager = userManager;
            this.mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<int>> Post(Evento evento)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            evento.Activo = true;
            context.Add(evento);
            await context.SaveChangesAsync(user.Id);
            return evento.EventoId;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Evento>> Get(int id)
        {
            var evento = await context.Eventos
               .Where(x => x.EventoId == id)
               .Include(x => x.Imagenes)
               .FirstOrDefaultAsync();
            if (evento == null) { return NotFound(); }

            //si el usuario no subio imagen poner una por defecto
            if (string.IsNullOrEmpty(evento.ImgEvento))
            {
                // Aquí colocas la URL de la imagen por defecto
                evento.ImgEvento = "Img" + "/" + "avatars-1.png";
            }
            
            return evento;
        }

        [HttpPut]
        public async Task<ActionResult> Put(Evento evento)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var eventoDB = await context.Eventos
                .Include(x => x.Imagenes)
                .FirstOrDefaultAsync(x => x.EventoId == evento.EventoId);

            if (eventoDB == null) { return NotFound(); }

            eventoDB = mapper.Map(evento, eventoDB);

            if (!string.IsNullOrWhiteSpace(evento.ImgEvento))
            {
                eventoDB.ImgEvento = evento.ImgEvento;
            }

            //recorremos las imagenes 
            foreach (var item in evento.Imagenes)
            {
                //si viene una imagen el usuario quiso editarla sino viene el usuario no quiso editar la imagen solo lo demas, y aplicamos el ignore de automapper
                if (!string.IsNullOrWhiteSpace(item.Imagen))
                {
                    eventoDB.Imagenes = evento.Imagenes;
                }
            }

            await context.SaveChangesAsync(user.Id);
            return NoContent();
        }       

        //utilizamos fromquery para traer los parametros de busqueda
        [HttpGet("filtrar")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Evento>>> Get([FromQuery] ParametrosBusqueda parametrosBusqueda)
        {
            bool mostrar = true;
            //query para traer los eventos
            var queryable = context.Eventos.Where(x => x.Activo == mostrar).OrderBy(x => x.EventoId).AsQueryable();

            if (parametrosBusqueda.EventoId != 0)
            {
                queryable = queryable.Where(x => x.EventoId == parametrosBusqueda.EventoId);
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
            public int EventoId { get; set; }           
            public bool Activo { get; set; }
        }

        //buscar facturas para asignarla a un equipo
        [HttpGet("buscar/{textoBusqueda}")]
        public async Task<ActionResult<List<Evento>>> GetEvento(string textoBusqueda)
        {
            if (textoBusqueda.Length > 3)
            {
                if (string.IsNullOrWhiteSpace(textoBusqueda)) { return new List<Evento>(); }
                textoBusqueda = textoBusqueda.ToLower();
                return await context.Eventos.Where(x => x.NombreEvento.ToLower().Contains(textoBusqueda)).Take(25).ToListAsync();
            }
            else
            {
                if (string.IsNullOrWhiteSpace(textoBusqueda)) { return new List<Evento>(); }
                textoBusqueda = textoBusqueda.ToLower();
                return await context.Eventos.Where(x => x.NombreEvento.ToLower().Contains(textoBusqueda)).Take(5).ToListAsync();
            }
        }

        [Route("Desactivar")]
        [HttpPut]
        public async Task<ActionResult> PutDesactivar(Evento evento)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            //obtener el registro original usando el método FindAsync 
            var oldevento = await context.Eventos.FindAsync(evento.EventoId);

            //las propiedades sin cambios se ignoran y solo los valores de cambios se incluyen en la consulta de actualización
            context.Entry(oldevento).CurrentValues.SetValues(evento);

            await context.SaveChangesAsync(user.Id);
            return NoContent();
        }

        [Route("ListadeEventosSencilla")]
        [HttpGet]
        public async Task<ActionResult<List<Evento>>> GetEvent()
        {
            //fijar una fecha en este caso una fecha de inicio
            DateTime FechaActual = DateTime.Now;
            DateTime FechaInicio = new DateTime(FechaActual.Year, FechaActual.Month, 1);

            //obtener el total de dias de un mes en este caso el actual
            int DiaFinMes = DateTime.DaysInMonth(FechaActual.Year, FechaActual.Month);
            DateTime FechaFinal = new DateTime(FechaActual.Year, FechaActual.Month, DiaFinMes);

            var listadeeventos = await context.Eventos.
                Where(
                x => x.Activo == true &&
                x.FechaEvento >= FechaInicio && x.FechaEvento <= FechaFinal
                ).ToListAsync();
            return listadeeventos;
        }
    }
}
