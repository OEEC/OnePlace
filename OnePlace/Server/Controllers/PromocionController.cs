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
using System.Threading.Tasks;

namespace OnePlace.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrador,Usuario")]
    public class PromocionController : ControllerBase
    {
        private readonly oneplaceContext context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper mapper;
        public PromocionController(oneplaceContext context, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            this.context = context;
            _userManager = userManager;
            this.mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<int>> Post(Promocion promocion)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);  
            promocion.Activo = true;
            promocion.LugardeVisualizacion = LugardeVisualizacion.PantallaPrincipal;
            context.Add(promocion);
            await context.SaveChangesAsync(user.Id);
            return promocion.PromocionId;
        }

        //utilizamos fromquery para traer los parametros de busqueda
        [HttpGet("filtrar")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Promocion>>> Get([FromQuery] ParametrosBusqueda parametrosBusqueda)
        {
            bool mostrar = true;           

            //List<Promocion> promociones = (from p in context.Promociones
            //                            select new Promocion
            //                            {
            //                                PromocionId = p.PromocionId,
            //                                Nombre = p.Nombre,
            //                                Descripcion = p.Descripcion,
            //                                FechadeRegistro = p.FechadeRegistro,
            //                                FechadeTermino = p.FechadeTermino,
            //                                TipodePromociones = p.TipodePromociones,
            //                                LugardeVisualizacion = p.LugardeVisualizacion,
            //                                Activo = p.Activo,
            //                                Mostrar = p.Mostrar,
            //                                Idzona = p.Idzona,
            //                                Zona = context.Zonas.Where(x => x.Idzona == p.Idzona).FirstOrDefault()
            //                            }).ToList();

            //query para traer las promociones
            var queryable = context.Promociones.Where(x => x.Activo == mostrar)
                .Include(x=>x.PromocionZona).ThenInclude(x=>x.Zona)
                .OrderBy(x => x.PromocionId).AsQueryable();             

            if (parametrosBusqueda.PromocionId != 0)
            {
                queryable = queryable.Where(x => x.PromocionId == parametrosBusqueda.PromocionId);
            }
            if (parametrosBusqueda.Activo == true)
            {
                //queryable = queryable.Where(x => x.Activo == false);
                mostrar = false;
            }
            if (parametrosBusqueda.ZonaId != 0)
            {
                queryable = queryable
                    .Where(x => x.PromocionZona.Select(y => y.ZonaId)
                    .Contains(parametrosBusqueda.ZonaId));
            }

            //paginacion
            await HttpContext.InsertarParametrosPaginacionEnRespuesta(queryable, parametrosBusqueda.CantidadRegistros, true);
            var promos = queryable.Paginar(parametrosBusqueda.Paginacion).ToList();
            return promos;
        }
        public class ParametrosBusqueda
        {
            public int Pagina { get; set; } = 1;
            public int CantidadRegistros { get; set; } = 10;
            public PaginacionDTO Paginacion
            {
                get { return new PaginacionDTO() { Pagina = Pagina, CantidadRegistros = CantidadRegistros }; }
            }
            public int ZonaId { get; set; }
            public int PromocionId { get; set; }
            public bool Activo { get; set; }
        }

        [HttpGet("{id}")]       
        public async Task<ActionResult<PromocionVisualizarDTO>> Get(int id)
        {
            var promocion = await context.Promociones.Where(x => x.PromocionId == id)
                .Include(x => x.Imagenes)
                .Include(x => x.PromocionZona).ThenInclude(x => x.Zona)               
                .FirstOrDefaultAsync();

            if (promocion == null) { return NotFound(); }    

            var model = new PromocionVisualizarDTO();
            model.Promocion = promocion;
            model.Zonas = promocion.PromocionZona.Select(x => x.Zona).ToList();        
            return model;
        }

        [HttpGet("Actualizar/{id}")]
        public async Task<ActionResult<PromocionZonaActualizacionDTO>> PutGet(int id)
        {
            var promocionActionResult = await Get(id);
            if (promocionActionResult.Result is NotFoundResult) { return NotFound(); }

            var promocionVisualizarDTO = promocionActionResult.Value;
            var zonasSeleccionadasIds = promocionVisualizarDTO.Zonas.Select(x => x.ZonaId).ToList();
            var zonasNoSeleccionadas = await context.Zonas
                .Where(x => !zonasSeleccionadasIds.Contains(x.ZonaId))
                .ToListAsync();

            var model = new PromocionZonaActualizacionDTO();
            model.Promocion = promocionVisualizarDTO.Promocion;
            model.ZonasNoSeleccionadas = zonasNoSeleccionadas;
            model.ZonasSeleccionadas = promocionVisualizarDTO.Zonas;           
            return model;
        }

        [HttpPut]
        public async Task<ActionResult> Put(Promocion promo)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var promoDB = await context.Promociones
                .Include(x => x.Imagenes)
                .FirstOrDefaultAsync(x => x.PromocionId == promo.PromocionId);

            if (promoDB == null) { return NotFound(); }

            promoDB = mapper.Map(promo, promoDB);

            //recorremos las imagenes 
            foreach (var item in promo.Imagenes)
            {
                //si viene una imagen el usuario quiso editarla sino viene el usuario no quiso editar la imagen solo lo demas, y aplicamos el ignore de automapper
                if (!string.IsNullOrWhiteSpace(item.Imagen))
                {
                    promoDB.Imagenes = promo.Imagenes;
                }
            }

            await context.Database.ExecuteSqlInterpolatedAsync($"delete from PromocionZonas WHERE PromocionId = {promo.PromocionId};");

           
            promoDB.PromocionZona = promo.PromocionZona;           

            await context.SaveChangesAsync(user.Id);
            return NoContent();
        }

        [Route("Desactivar")]
        [HttpPut]
        public async Task<ActionResult> PutDesactivar(Promocion promo)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            //obtener el registro original usando el método FindAsync 
            var oldpromo = await context.Promociones.FindAsync(promo.PromocionId);

            //las propiedades sin cambios se ignoran y solo los valores de cambios se incluyen en la consulta de actualización
            context.Entry(oldpromo).CurrentValues.SetValues(promo);

            await context.SaveChangesAsync(user.Id);
            return NoContent();
        }

        //buscar facturas para asignarla a un equipo
        [HttpGet("buscar/{textoBusqueda}")]
        public async Task<ActionResult<List<Promocion>>> GetPromocion(string textoBusqueda)
        {
            if (textoBusqueda.Length > 3)
            {
                if (string.IsNullOrWhiteSpace(textoBusqueda)) { return new List<Promocion>(); }
                textoBusqueda = textoBusqueda.ToLower();
                return await context.Promociones.Where(x => x.Nombre.ToLower().Contains(textoBusqueda)).ToListAsync();
            }
            else
            {
                if (string.IsNullOrWhiteSpace(textoBusqueda)) { return new List<Promocion>(); }
                textoBusqueda = textoBusqueda.ToLower();
                return await context.Promociones.Where(x => x.Nombre.ToLower().Contains(textoBusqueda)).Take(50).ToListAsync();
            }
        }

        [Route("ListaZonaSencilla")]
        [HttpGet]
        public async Task<ActionResult<List<Zona>>> GetEvent()
        {
            var listadezonas = await context.Zonas.ToListAsync();              
            return listadezonas;
        }

        [Route("ListaPromocionSencilla")]
        [HttpGet]
        public async Task<ActionResult<List<Promocion>>> GetPromo()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            //buscamos un empleado por el usuario logueado
            var empleado = await context.Empleados.Where(x => x.Idempleado == user.Idempleado).FirstOrDefaultAsync();

            //buscamos las zonas relacionadas al empleado
            var listapromocionzona = await context.PromocionZonas.Where(x => x.ZonaId == empleado.ZonaId).ToListAsync();

            List<Promocion> listadepromocionescarrusel = new List<Promocion>();

            var FechadeHoy = DateTime.Today;

            //recorremos las zonas ligadas a una promocion, para obtener las promociones por zona
            foreach(var item in listapromocionzona)
            {
                var promocion = await context.Promociones.Where(x => x.PromocionId == item.PromocionId && x.Activo == true)
                    .Include(x => x.PromocionZona).ThenInclude(x=>x.Zona)
                    .Include(x => x.Imagenes)
                    .FirstOrDefaultAsync();

                //solo agregar las promociones que no se han vencido
                if(FechadeHoy >= promocion.FechadeTermino)
                {

                }
                else
                {
                    listadepromocionescarrusel.Add(promocion);
                }               
            }
            
            return listadepromocionescarrusel;
        }
    }
}
