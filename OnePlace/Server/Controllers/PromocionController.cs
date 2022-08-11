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
            if (promocion.Zona != null)
            {
                promocion.ZonaId = promocion.Zona.ZonaId;
                promocion.Zona = null;
            }
            else
            {
                string mensajeError = "Debe seleccionar una zona";
                return BadRequest(mensajeError);
            }
          
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
                .Include(x=>x.Zona)
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
            public int PromocionId { get; set; }
            public bool Activo { get; set; }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Promocion>> GetPromo(int id)
        {
            //Promocion promocion = await (from p in context.Promociones
            //                             where p.PromocionId == id
            //                             select new Promocion
            //                             {
            //                                 PromocionId = p.PromocionId,
            //                                 Nombre = p.Nombre,
            //                                 Descripcion = p.Descripcion,
            //                                 FechadeRegistro = p.FechadeRegistro,
            //                                 FechadeTermino = p.FechadeTermino,
            //                                 TipodePromociones = p.TipodePromociones,
            //                                 LugardeVisualizacion = p.LugardeVisualizacion,
            //                                 Activo = p.Activo,
            //                                 Mostrar = p.Mostrar,
            //                                 Idzona = p.Idzona,
            //                                 Zona = context.Zonas.Where(x => x.Idzona == p.Idzona).FirstOrDefault(),
            //                                 Imagenes = context.ImagenesCarruseles.Where(x => x.PromocionId == p.PromocionId).ToList()
            //                             }).FirstOrDefaultAsync();

            var promocion = await context.Promociones
               .Where(x => x.PromocionId == id)
               .Include(x => x.Imagenes)
               .Include(x => x.Zona)
               .FirstOrDefaultAsync();

            if (promocion == null) { return NotFound(); }
            return promocion;
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
                return await context.Promociones.Where(x => x.Nombre.ToLower().Contains(textoBusqueda)).Take(25).ToListAsync();
            }
            else
            {
                if (string.IsNullOrWhiteSpace(textoBusqueda)) { return new List<Promocion>(); }
                textoBusqueda = textoBusqueda.ToLower();
                return await context.Promociones.Where(x => x.Nombre.ToLower().Contains(textoBusqueda)).Take(5).ToListAsync();
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
            var empleado = await context.Empleados.Where(x => x.Idempleado == user.Idempleado).FirstOrDefaultAsync();

            var carrusel = await context.Promociones.Where(x => x.Activo == true && x.ZonaId == empleado.ZonaId)
                .Include(x => x.Imagenes).ToListAsync();
            if (carrusel == null) { return NotFound(); }
            return carrusel;
        }
    }
}
