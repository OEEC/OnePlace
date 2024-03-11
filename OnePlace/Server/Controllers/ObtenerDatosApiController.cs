using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OnePlace.Client.ComponentesGenericos.Listado;
using OnePlace.Client.Service;
using OnePlace.Server.Data;
using OnePlace.Shared.Entidades.SimsaCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace OnePlace.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrador")]
    public class ObtenerDatosApiController : ControllerBase
    {
        private readonly oneplaceContext context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ISimsacoreService simsacoreService;
        public ObtenerDatosApiController(oneplaceContext context, UserManager<ApplicationUser> userManager, ISimsacoreService simsacoreService)
        {
            this.context = context;
            _userManager = userManager;
            this.simsacoreService = simsacoreService;
        }

        [Route("ActualizarPersona")]
        [HttpPost]
        public async Task<ActionResult> PostPersona([FromQuery] int? persona)
        {
            if (persona is not null)
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);

                var resultadoapirest = await simsacoreService.GetAllPersonas();
                var persona_empleado = resultadoapirest.ListadePersonas.FirstOrDefault(x => x.Idpersona == persona);

                if (persona_empleado is not null)
                {
                    if (CultureInfo.InvariantCulture.Calendar.IsLeapYear(persona_empleado.Fchnac.Value.Year))
                        persona_empleado.Fchnac = new DateTime(persona_empleado.Fecha_Nacimiento.Value.Year, persona_empleado.Fecha_Nacimiento.Value.Month, persona_empleado.Fecha_Nacimiento.Value.Day);
                    else
                        persona_empleado.Fchnac = new DateTime(persona_empleado.Fecha_Nacimiento.Value.Year, persona_empleado.Fecha_Nacimiento.Value.Month, persona_empleado.Fecha_Nacimiento.Value.Day);
                }

                context.Add(persona_empleado);
                await context.SaveChangesAsync(user.Id);
                return Ok();
            }
            //var user = await _userManager.GetUserAsync(HttpContext.User);

            //var resultadoapirest = await simsacoreService.GetAllPersonas();
            //var persona_empleado = resultadoapirest.ListadePersonas.FirstOrDefault(x => x.Idpersona == persona);

            //if (persona_empleado is not null)
            //{
            //    if (CultureInfo.InvariantCulture.Calendar.IsLeapYear(persona_empleado.Fchnac.Value.Year))
            //        persona_empleado.Fchnac = new DateTime(persona_empleado.Fecha_Nacimiento.Year, persona_empleado.Fecha_Nacimiento.Month, persona_empleado.Fecha_Nacimiento.Day);
            //    else
            //        persona_empleado.Fchnac = new DateTime(persona_empleado.Fecha_Nacimiento.Year, persona_empleado.Fecha_Nacimiento.Month, persona_empleado.Fecha_Nacimiento.Day);
            //}

            //context.Add(persona_empleado);

            //List<Persona> listadepersonas = new List<Persona>();

            //foreach (var item in resultadoapirest.ListadePersonas)
            //{
            //    //Persona persona = new Persona();
            //    //persona.Idpersona = item.Idpersona;
            //    //persona.ApePat = item.ApePat;
            //    //persona.ApeMat = item.ApeMat;
            //    //persona.Nombre = item.Nombre;
            //    //persona.Sexo = item.Sexo;

            //    if (CultureInfo.InvariantCulture.Calendar.IsLeapYear(item.Fchnac.Value.Year))
            //        item.Fchnac = new DateTime(item.Fecha_Nacimiento.Year, item.Fecha_Nacimiento.Month, item.Fecha_Nacimiento.Day);
            //    else
            //        item.Fchnac = new DateTime(item.Fecha_Nacimiento.Year, item.Fecha_Nacimiento.Month, item.Fecha_Nacimiento.Day);

            //    //si la fecha de nacimiento viene null guarda la fecha en 0000-00-00
            //    //if (item.Fchnac == null)
            //    //{
            //    //    persona.Fchnac = new DateTime(0001, 01, 01); //no se puede guardar 0000-00-00 da error
            //    //}
            //    //else
            //    //{
            //    //sino viene null debes separar la fecha en variables por que la bd espera un datetime sin aceptar null y despues crear una nueva fecha que no permita null
            //    //la clase donde se deserealiza la api person si permite null,
            //    //permite null por que para deserealizar la fecha viene 0000-00-00 y lo toma como string luego ya no puede convertir string en datetime (paso lo mismo en areas)

            //    //int dia = 0;
            //    //int mes = 0;
            //    //int ano = 0;

            //    //por cada fecha de nacimientos obtenemos el año
            //    //int year = Convert.ToInt32(item.Fchnac.Value.Year);

            //    //una vez obtenido el año verificamos si el año es biciesto
            //    //if (year % 400 == 0 || (year % 4 == 0 && year % 100 != 0))
            //    //{
            //    //    dia = Convert.ToInt32(item.Fchnac.Value.Day);
            //    //    mes = Convert.ToInt32(item.Fchnac.Value.Month);
            //    //    ano = Convert.ToInt32(item.Fchnac.Value.Year);

            //    //    /*si es biciesto nos permite crear una fecha fuera de rango o inexistente como un 29 de febrero
            //    //      sino verificamos que el año es biciesto da un error:Year, Month, and Day parameters describe an un-representable DateTime
            //    //      ese error es por que no puedes crear una fecha que no existe por eso verificamos si el año es biciesto*/
            //    //    var FechaBiciesta = new DateTime(year, mes, dia);
            //    //    persona.Fchnac = FechaBiciesta;
            //    //}
            //    //else
            //    //{
            //    //    dia = Convert.ToInt32(item.Fchnac.Value.Day);
            //    //    mes = Convert.ToInt32(item.Fchnac.Value.Month);
            //    //    ano = Convert.ToInt32(item.Fchnac.Value.Year);

            //    //    var fechasinnull = new DateTime(year, mes, dia);
            //    //    persona.Fchnac = fechasinnull;
            //    //}
            //    //}

            //    //persona.Rfc = item.Rfc;
            //    //persona.Curp = item.Curp;
            //    //persona.Nss = item.Nss;
            //    //persona.Calle = item.Calle;
            //    //persona.Noext = item.Noext;
            //    //persona.Noint = item.Noint;
            //    //persona.Colonia = item.Colonia;
            //    //persona.Cp = item.Cp;
            //    //persona.Ciudad = item.Ciudad;
            //    //persona.Estado = item.Estado;
            //    //persona.Correo = item.Correo;
            //    //persona.Telefono = item.Telefono;

            //    var existe = context.Personas.Any(x => x.Idpersona == item.Idpersona);
            //    if (!existe)
            //        listadepersonas.Add(item);
            //}

            //context.Personas.AddRange(listadepersonas);
            //await context.SaveChangesAsync(user.Id);
            return Ok();
        }

        [Route("ActualizarEmpleado")]
        [HttpPost]
        public async Task<ActionResult> PostEmpleado([FromQuery] int zona)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var resultadoapirest = await simsacoreService.GetAllEmpleados();
            var personas = await simsacoreService.GetAllPersonas();
            var areas = await simsacoreService.GetAllAreas();
            var departamento = await simsacoreService.GetAllDepartamentos();
            var empresas = await simsacoreService.GetAllRazonesSociales();
            var estaciones = await simsacoreService.GetAllEstaciones();

            List<Area> listado_areas = new();
            List<Empleado> listadeempleados = new();
            List<Persona> listado_personas = new();
            List<Departamento> listado_departamentos = new();
            List<Empresa> listado_empresa = new();
            List<Estacion> listado_estacion = new();

            foreach (var item in resultadoapirest.ListadeEmpleados.Where(x => x.ZonaId == zona))
            {
                if (!context.Empleados.Any(x => x.Idempleado == item.Idempleado))
                {
                    if (item.Idpersona is not null)
                    {
                        if (!context.Personas.Any(x => x.Idpersona == item.Idpersona))
                        {
                            var persona_empleado = personas.ListadePersonas.FirstOrDefault(x => x.Idpersona == item.Idpersona);
                            if (persona_empleado is not null)
                            {
                                if (persona_empleado.Fecha_Nacimiento is not null)
                                {
                                    if (CultureInfo.InvariantCulture.Calendar.IsLeapYear(persona_empleado.Fchnac.Value.Year))
                                        persona_empleado.Fchnac = new DateTime(persona_empleado.Fecha_Nacimiento.Value.Year, persona_empleado.Fecha_Nacimiento.Value.Month, persona_empleado.Fecha_Nacimiento.Value.Day);
                                    else
                                        persona_empleado.Fchnac = new DateTime(persona_empleado.Fecha_Nacimiento.Value.Year, persona_empleado.Fecha_Nacimiento.Value.Month, persona_empleado.Fecha_Nacimiento.Value.Day);
                                }

                                listado_personas.Add(persona_empleado);
                            }
                        }

                    }

                    if (item.Iddepartamento is not null)
                    {
                        if (!context.Departamentos.Any(x => x.Iddepartamento == item.Iddepartamento))
                        {
                            var departamento_empleado = departamento.ListadeDepartamentos.FirstOrDefault(x => x.Iddepartamento == item.Iddepartamento);
                            if (departamento_empleado is not null)
                            {

                                if (!string.IsNullOrEmpty(departamento_empleado.Nombre_Departamento))
                                {
                                    departamento_empleado.Departamento1 = departamento_empleado.Nombre_Departamento;

                                    listado_departamentos.Add(departamento_empleado);
                                }

                                if (departamento_empleado.Idempresa is not null)
                                {
                                    if (!context.Empresas.Any(x => x.Idempresa == departamento_empleado.Idempresa))
                                    {
                                        var empresa_dep = empresas.ListadeRazonesSociales.FirstOrDefault(x => x.Idempresa == departamento_empleado.Idempresa);
                                        if (empresa_dep is not null)
                                            listado_empresa.Add(empresa_dep);
                                    }

                                }
                            }
                        }
                    }

                    if (item.Idarea is not null)
                    {
                        if (!context.Areas.Any(x => x.Idarea == item.Idarea))
                        {
                            var area_empleado = areas.ListadeAreas.FirstOrDefault(x => x.Idarea == item.Idarea);
                            if (area_empleado is not null)
                            {
                                if (!string.IsNullOrEmpty(area_empleado.Nombre_Area))
                                {
                                    area_empleado.Area1 = area_empleado.Nombre_Area;
                                    listado_areas.Add(area_empleado);
                                }
                            }
                        }

                    }

                    if (item.Idestacion is not null)
                    {
                        if (!context.Estaciones.Any(x => x.Idestacion == item.Idestacion))
                        {
                            var estacion_empleado = estaciones.ListadeEstaciones.FirstOrDefault(x => x.Idestacion == item.Idestacion);
                            if (estacion_empleado is not null)
                                listado_estacion.Add(estacion_empleado);
                        }

                    }

                    if (item.Idestacion is not null)
                    {
                        if (!context.Estaciones.Any(x => x.Idestacion == item.Idestacion))
                        {
                            var estacion_empleado = estaciones.ListadeEstaciones.FirstOrDefault(x => x.Idestacion == item.Idestacion);
                            if (estacion_empleado is not null)
                                listado_estacion.Add(estacion_empleado);
                        }

                    }

                    if (item.Idestacion == 0)
                        item.Idestacion = null;

                    if (item.TiendaId == 0)
                        item.TiendaId = null;

                    listadeempleados.Add(item);
                }
                else
                {
                    var existe = context.Empleados.FirstOrDefault(x => x.Idempleado == item.Idempleado);
                    if (existe is not null && (existe.Fchalta != item.Fchalta || existe.Division != item.Division || existe.Fchbaja != item.Fchbaja))
                    {
                        existe.Fchalta = item.Fchalta;
                        existe.Division = item.Division;
                        existe.Fchbaja = item.Fchbaja;
                        context.Empleados.Update(existe);
                    }

                }
            }

            context.Empresas.AddRange(listado_empresa);
            await context.SaveChangesAsync(user.Id);

            context.Departamentos.AddRange(listado_departamentos);
            await context.SaveChangesAsync(user.Id);

            context.Areas.AddRange(listado_areas);
            await context.SaveChangesAsync(user.Id);

            context.Personas.AddRange(listado_personas);
            await context.SaveChangesAsync(user.Id);

            context.Estaciones.AddRange(listado_estacion);
            await context.SaveChangesAsync(user.Id);

            context.Empleados.AddRange(listadeempleados);
            await context.SaveChangesAsync(user.Id);

            return Ok();
        }

        [Route("ActualizarEstacion")]
        [HttpPost]
        public async Task<ActionResult> PostEstacion([FromQuery] int zona)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var resultadoapirest = await simsacoreService.GetAllEstaciones();

            List<Estacion> listadeestaciones = new List<Estacion>();

            foreach (var estacion in resultadoapirest.ListadeEstaciones)
                if (!context.Estaciones.Any(x => x.Idestacion == estacion.Idestacion))
                    listadeestaciones.Add(estacion);

            context.Estaciones.AddRange(listadeestaciones);
            await context.SaveChangesAsync(user.Id);
            return Ok();
        }

        [HttpPost("ActualizarZona")]
        public async Task<ActionResult> PostZona(Zona zona)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var results = await simsacoreService.GetAllZonas();

            List<Zona> lista = new();

            foreach (var _zona in results.Zonas)
            {
                if (!context.Zonas.Any(x => x.ZonaId == _zona.Id_Zona))
                {
                    _zona.Zona1 = _zona.Nombre_Zona;
                    _zona.ZonaId = _zona.Id_Zona;

                    lista.Add(_zona);
                }
            }
            context.AddRange(lista);
            await context.SaveChangesAsync(user.Id);

            return Ok();
        }

        [HttpPost("ActualizarAreas")]
        public async Task<ActionResult> PostArea(Area area)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var resultadoapirest = await simsacoreService.GetAllAreas();

            List<Area> listado_areas = new List<Area>();

            foreach (var _area in listado_areas)
            {
                _area.Area1 = _area.Nombre_Area;
                listado_areas.Add(_area);
            }

            context.Areas.AddRange(listado_areas);
            await context.SaveChangesAsync(user.Id);
            return Ok();
        }

        [HttpGet("ActualizarCatalogo")]
        public async Task<ActionResult> Actualizar_Catalogo()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var areas = await simsacoreService.GetAllAreas();
            var departamento = await simsacoreService.GetAllDepartamentos();
            var empresas = await simsacoreService.GetAllRazonesSociales();
            var puestos = await simsacoreService.GetAllPuestos();
            var zonas = await simsacoreService.GetAllZonas();

            List<Area> listado_areas = new();
            List<Departamento> listado_departamentos = new();
            List<Empresa> listado_empresa = new();
            List<Zona> listado_zonas = new();
            List<Puesto> listado_puestos = new();

            foreach (var zona in zonas.Zonas)
            {
                zona.Zona1 = zona.Nombre_Zona;
                zona.ZonaId = zona.Id_Zona;

                if (!context.Zonas.Any(x => x.ZonaId == zona.ZonaId))
                    listado_zonas.Add(zona);
                else
                    context.Update(zona);
            }

            foreach (var empresa in empresas.ListadeRazonesSociales)
                if (!context.Empresas.Any(x => x.Idempresa == empresa.Idempresa))
                    context.Add(empresa);
                else
                    context.Update(empresa);

            foreach (var dep in departamento.ListadeDepartamentos)
            {
                dep.Departamento1 = dep.Nombre_Departamento;
                if (!context.Departamentos.Any(x => x.Iddepartamento == dep.Iddepartamento))
                    listado_departamentos.Add(dep);
                else
                    context.Update(dep);
            }

            foreach (var area in areas.ListadeAreas)
            {
                area.Area1 = area.Nombre_Area;
                if (!context.Areas.Any(x => x.Idarea == area.Idarea))
                    listado_areas.Add(area);
                else
                    context.Update(area);
            }

            await context.SaveChangesAsync(user.Id);

            context.Empresas.AddRange(listado_empresa);
            await context.SaveChangesAsync(user.Id);

            context.Departamentos.AddRange(listado_departamentos);
            await context.SaveChangesAsync(user.Id);

            context.Areas.AddRange(listado_areas);
            await context.SaveChangesAsync(user.Id);

            context.Puestos.AddRange(listado_puestos);
            await context.SaveChangesAsync(user.Id);

            context.Zonas.AddRange(listado_zonas);
            await context.SaveChangesAsync(user.Id);

            return Ok(true);
        }

    }
}
