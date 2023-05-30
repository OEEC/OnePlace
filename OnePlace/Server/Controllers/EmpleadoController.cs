using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OnePlace.Server.Data;
using OnePlace.Server.Helpers;
using OnePlace.Shared.DTOs;
using OnePlace.Shared.Entidades.SimsaCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace OnePlace.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrador")]
    public class EmpleadoController : ControllerBase
    {
        private readonly oneplaceContext context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<EmpleadoController> logger;
        public EmpleadoController(oneplaceContext context, UserManager<ApplicationUser> userManager, ILogger<EmpleadoController> logger)
        {
            this.context = context;
            _userManager = userManager;
            this.logger = logger;
        }

        #region EmpleadoconLogger

        //[HttpGet]
        //public async Task<ActionResult<List<Empleado>>> Get()
        //{
        //    logger.LogWarning("Log al pedir empleados");

        //    //ejemplo con la excepcion incluida
        //    try
        //    {
        //        throw new NotImplementedException();
        //    }
        //    catch (NotImplementedException ex)
        //    {
        //        logger.LogError(ex, ex.Message);
        //    }

        //    var user = await _userManager.GetUserAsync(HttpContext.User);
        //    logger.LogError("{UserId} some message", user.Id);

        //    var listadodeempleados = await context.Empleados.ToListAsync();
        //    return listadodeempleados;
        //}

        #endregion

        #region GetEmpleadosinFiltros

        public class ParametrosBusqueda
        {
            public int Pagina { get; set; } = 1;
            public int CantidadRegistros { get; set; } = 12;
            public PaginacionDTO Paginacion
            {
                get { return new PaginacionDTO() { Pagina = Pagina, CantidadRegistros = CantidadRegistros }; }
            }
            public int EmpleadoId { get; set; }
            public int DepartamentoId { get; set; }
            public string Division { get; set; }
            public bool Activo { get; set; }

            public int zona { get; set; }

        }

        //[HttpGet]
        //public async Task<ActionResult<List<Empleado>>> GetEmp([FromQuery] PaginacionDTO paginacion)
        //{    
        //    //para obtener una union de dos tablas que no estan unidas por fk, se realiza una consulta linq
        //    //se crea un nuevo objeto empleado pero se tiene que poner toda su data y como propiedad de navegacion
        //    //se hace otra consulta para obtener la data persona por que no se puede usar el include
        //    List<Empleado> empleados = (from e in context.Empleados
        //                               select new Empleado
        //                               {
        //                                   Idempleado = e.Idempleado,
        //                                   Idpersona = e.Idpersona,
        //                                   Img = e.Img,
        //                                   Noemp = e.Noemp,
        //                                   Correo = e.Correo,
        //                                   Telefono = e.Telefono,
        //                                   Iddepartamento = e.Iddepartamento,
        //                                   Idarea = e.Idarea,
        //                                   Idpuesto = e.Idpuesto,
        //                                   Nomina = e.Nomina,
        //                                   Variable = e.Variable,
        //                                   Idtipo = e.Idtipo,
        //                                   Fchalta = e.Fchalta,
        //                                   Fchbaja = e.Fchbaja,
        //                                   Persona = context.Personas.Where(x => x.Idpersona == e.Idpersona).FirstOrDefault(),
        //                                   Departamento = context.Departamentos.Where(x => x.Iddepartamento == e.Iddepartamento).FirstOrDefault(),
        //                                   Area = context.Areas.Where(x => x.Idarea == e.Idarea).FirstOrDefault(),
        //                                   Puesto = context.Puestos.Where(x => x.Idpuesto == e.Idpuesto).FirstOrDefault()
        //                               }).ToList();

        //    foreach (var item in empleados)
        //    {
        //        if (string.IsNullOrEmpty(item.Img))
        //        {
        //            // Aquí colocas la URL de la imagen por defecto
        //            item.Img = "Img" + "/" + "usuario.png";
        //        }
        //    }

        //    var queryable = empleados.AsQueryable();
        //    await HttpContext.InsertarParametrosPaginacionEnRespuesta(queryable, paginacion.CantidadRegistros,true);
        //    return queryable.Paginar(paginacion).ToList();
        //}

        #endregion

        [HttpGet("{id}")]
        public async Task<ActionResult<EmpleadoPersonaDTO>> Get(int id)
        {
            Empleado empleado = (from e in context.Empleados where e.Idempleado == id
                                        select new Empleado
                                        {
                                            Idempleado = e.Idempleado,
                                            Idpersona = e.Idpersona,
                                            Img = e.Img,
                                            Noemp = e.Noemp,
                                            Correo = e.Correo,
                                            Telefono = e.Telefono,
                                            Iddepartamento = e.Iddepartamento,
                                            Idarea = e.Idarea,
                                            Idpuesto = e.Idpuesto,
                                            Nomina = e.Nomina,
                                            Variable = e.Variable,
                                            Idtipo = e.Idtipo,
                                            Fchalta = e.Fchalta,
                                            Fchbaja = e.Fchbaja,
                                            Division = e.Division,
                                            ImagenesCumple = context.ImagenesCumpleEmpleado.Where(x=>x.EmpleadoId == e.Idempleado).ToList(),
                                            Departamento = context.Departamentos.Where(x => x.Iddepartamento == e.Iddepartamento).FirstOrDefault(),
                                            Area = context.Areas.Where(x => x.Idarea == e.Idarea).FirstOrDefault(),
                                            Puesto = context.Puestos.Where(x => x.Idpuesto == e.Idpuesto).FirstOrDefault()
                                        })                                       
                                        .FirstOrDefault();

            if (empleado == null) { return NotFound(); }

            //si el usuario no subio imagen poner una por defecto
            if (string.IsNullOrEmpty(empleado.Img))
            {
                // Aquí colocas la URL de la imagen por defecto
                empleado.Img = "Img" + "/" + "avatars-1.png";
            }

            var persona = await context.Personas.Where(x => x.Idpersona == empleado.Idpersona).FirstOrDefaultAsync();
            if (persona == null) { return NotFound(); }           

            var model = new EmpleadoPersonaDTO();
            model.Empleado = empleado;
            model.Persona = persona;

            return model;
        }

        //utilizamos fromquery para traer los parametros de busqueda
        [HttpGet("filtrar")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Empleado>>> Get([FromQuery] ParametrosBusqueda parametrosBusqueda)
        {
            bool mostrar = true;

            //para obtener una union de dos tablas que no estan unidas por fk, se realiza una consulta linq
            //se crea un nuevo objeto empleado pero se tiene que poner toda su data y como propiedad de navegacion
            //se hace otra consulta para obtener la data persona por que no se puede usar el include
            List<Empleado> empleados = (from e in context.Empleados
                                        select new Empleado
                                        {
                                            Idempleado = e.Idempleado,
                                            Idpersona = e.Idpersona,
                                            Img = e.Img,
                                            Noemp = e.Noemp,
                                            Correo = e.Correo,
                                            Telefono = e.Telefono,
                                            Iddepartamento = e.Iddepartamento,
                                            Idarea = e.Idarea,
                                            Idpuesto = e.Idpuesto,
                                            Nomina = e.Nomina,
                                            Variable = e.Variable,
                                            Idtipo = e.Idtipo,
                                            Fchalta = e.Fchalta,
                                            Fchbaja = e.Fchbaja,
                                            Division = e.Division,
                                            ZonaId = e.ZonaId,
                                            Persona = context.Personas.Where(x => x.Idpersona == e.Idpersona).FirstOrDefault(),
                                            Departamento = context.Departamentos.Where(x => x.Iddepartamento == e.Iddepartamento).FirstOrDefault(),
                                            Area = context.Areas.Where(x => x.Idarea == e.Idarea).FirstOrDefault(),
                                            Puesto = context.Puestos.Where(x => x.Idpuesto == e.Idpuesto).FirstOrDefault(),
                                        }).ToList();          

            var queryable = empleados.OrderBy(x => x.Idpersona).AsQueryable();

            if (parametrosBusqueda.EmpleadoId != 0)
            {
                queryable = queryable.Where(x => x.Idempleado == parametrosBusqueda.EmpleadoId);
            }
            if (parametrosBusqueda.DepartamentoId != 0)
            {
                queryable = queryable.Where(x => x.Iddepartamento == parametrosBusqueda.DepartamentoId);
            }
            if (parametrosBusqueda.zona != 0)
            {
                queryable = queryable.Where(x => x.ZonaId == parametrosBusqueda.zona);
            }
            if (!string.IsNullOrEmpty(parametrosBusqueda.Division))
            {
                queryable = queryable.Where(x => x.Division != null && x.Division.ToLower().Contains(parametrosBusqueda.Division.ToLower()));
            }
            //if (parametrosBusqueda.Activo == true)
            //{
            //    mostrar = false;
            //    //queryable = queryable.Where(x => x.Activo == mostrar);               
            //}

            //si el usuario no subio imagen poner una por defecto
            foreach (var item in queryable)
            {
                if (string.IsNullOrEmpty(item.Img))
                {
                    // Aquí colocas la URL de la imagen por defecto
                    item.Img = "Img" + "/" + "usuario.png";
                }
            }
            //paginacion
            await HttpContext.InsertarParametrosPaginacionEnRespuesta(queryable, parametrosBusqueda.CantidadRegistros,true);
            var listaARetornar = queryable.Paginar(parametrosBusqueda.Paginacion).ToList();
            return listaARetornar;
        }

        //buscar empleados para filtro
        [HttpGet("buscar/{textoBusqueda}")]
        public async Task<ActionResult<List<Empleado>>> GetEmpleado(string textoBusqueda)
        {
            if (textoBusqueda.Length > 3)
            {
                if (string.IsNullOrWhiteSpace(textoBusqueda)) { return new List<Empleado>(); }
                textoBusqueda = textoBusqueda.ToLower();
                
                List<Empleado> empleados = await (from e in context.Empleados
                                            select new Empleado
                                            {
                                                Idempleado = e.Idempleado,
                                                Idpersona = e.Idpersona,
                                                Img = e.Img,
                                                Noemp = e.Noemp,
                                                Correo = e.Correo,
                                                Telefono = e.Telefono,
                                                Iddepartamento = e.Iddepartamento,
                                                Idarea = e.Idarea,
                                                Idpuesto = e.Idpuesto,
                                                Nomina = e.Nomina,
                                                Variable = e.Variable,
                                                Idtipo = e.Idtipo,
                                                Fchalta = e.Fchalta,
                                                Fchbaja = e.Fchbaja,
                                                Division = e.Division,
                                                Persona = context.Personas.Where(x => x.Idpersona == e.Idpersona).FirstOrDefault(),                                              
                                            })
                                            .Where(x => x.Persona.Nombre.ToLower().Contains(textoBusqueda) || x.Persona.ApePat.ToLower().Contains(textoBusqueda) || x.Noemp.ToLower().Contains(textoBusqueda))                                            
                                            .ToListAsync();

                return empleados;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(textoBusqueda)) { return new List<Empleado>(); }
                textoBusqueda = textoBusqueda.ToLower();              

                List<Empleado> empleados = await (from e in context.Empleados
                                            select new Empleado
                                            {
                                                Idempleado = e.Idempleado,
                                                Idpersona = e.Idpersona,
                                                Img = e.Img,
                                                Noemp = e.Noemp,
                                                Correo = e.Correo,
                                                Telefono = e.Telefono,
                                                Iddepartamento = e.Iddepartamento,
                                                Idarea = e.Idarea,
                                                Idpuesto = e.Idpuesto,
                                                Nomina = e.Nomina,
                                                Variable = e.Variable,
                                                Idtipo = e.Idtipo,
                                                Fchalta = e.Fchalta,
                                                Fchbaja = e.Fchbaja,
                                                Division = e.Division,
                                                Persona = context.Personas.Where(x => x.Idpersona == e.Idpersona).FirstOrDefault(),
                                            })
                                           .Where(x => x.Persona.Nombre.ToLower().Contains(textoBusqueda) || x.Persona.ApePat.ToLower().Contains(textoBusqueda) || x.Noemp.ToLower().Contains(textoBusqueda))
                                           .Take(50)
                                           .ToListAsync();

                return empleados;
            }
        }

        [Route("ImgCumpleEmpleado")]
        [HttpPost]
        public async Task<ActionResult<int>> Post(Empleado empleado)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            List<ImagenesCumpleEmpleado> listadeimagenes = new List<ImagenesCumpleEmpleado>();

            //recorremos la lsita de imagenes que trae la data del emmpleado, y por cada iteracion se crea un objeto 
            //ImagenesCumpleEmpleado y le asignamos el idempleado ya que el que viene en la data no lo trae
            //aqui es donde hacemos la relacion empleado-imagenescumpleempleado
            foreach (var item in empleado.ImagenesCumple)
            {
                ImagenesCumpleEmpleado imagen = new ImagenesCumpleEmpleado();
                imagen.EmpleadoId = empleado.Idempleado;
                imagen.Imagen = item.Imagen;

                listadeimagenes.Add(imagen);
            }

            context.ImagenesCumpleEmpleado.AddRange(listadeimagenes);
            await context.SaveChangesAsync(user.Id);
            return empleado.Idempleado;
        }
    }
}
