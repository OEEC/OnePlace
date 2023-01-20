using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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

        //[HttpPost]
        //public async Task<ActionResult> Post(Zona zzona)
        //{
        //    var user = await _userManager.GetUserAsync(HttpContext.User);

        //    var resultadoapirest = await simsacoreService.GetAllZonas();

        //    List<Zona> listadezonas = new List<Zona>();

        //    foreach (var item in resultadoapirest.Zonas)
        //    {
        //        Zona zona = new Zona();
        //        zona.ZonaId = item.ZonaId;
        //        zona.Zona1 = item.Zona1;
        //        zona.Idestatus = item.Idestatus;
        //        zona.Fchcreacion = item.Fchcreacion;
        //        zona.Fchmod = item.Fchmod;
        //        zona.Fchbaja = item.Fchbaja;
        //        zona.Idusuario = item.Idusuario;

        //        var existe = await context.Zonas.AnyAsync(x => x.ZonaId == item.ZonaId);
        //        if (!existe)
        //        {
        //            listadezonas.Add(zona);
        //        }
        //    }

        //    context.Zonas.AddRange(listadezonas);
        //    await context.SaveChangesAsync(user.Id);
        //    return Ok();
        //}

        //[HttpPost]
        //public async Task<ActionResult> Post(Empresa empresa1)
        //{
        //    var user = await _userManager.GetUserAsync(HttpContext.User);

        //    var resultadoapirest = await simsacoreService.GetAllRazonesSociales();

        //    List<Empresa> listadeempresas = new List<Empresa>();

        //    foreach (var item in resultadoapirest.ListadeRazonesSociales)
        //    {
        //        Empresa empresa = new Empresa();
        //        empresa.Idempresa = item.Idempresa;
        //        empresa.Rfc = item.Rfc;
        //        empresa.Patronal = item.Patronal;
        //        empresa.Razonsocial = item.Domicilio;
        //        empresa.Idestatus = item.Idestatus;
        //        empresa.Fchcreacion = item.Fchcreacion;
        //        empresa.Fchmod = item.Fchmod;
        //        empresa.Fchbaja = item.Fchbaja;
        //        empresa.Idusuario = item.Idusuario;

        //        var existe = await context.Empresas.AnyAsync(x => x.Idempresa == item.Idempresa);
        //        if (!existe)
        //        {
        //            listadeempresas.Add(empresa);
        //        }
        //    }

        //    context.Empresas.AddRange(listadeempresas);
        //    await context.SaveChangesAsync(user.Id);
        //    return Ok();
        //}

        //[HttpPost]
        //public async Task<ActionResult> Post(Marca marca1)
        //{
        //    var user = await _userManager.GetUserAsync(HttpContext.User);

        //    var resultadoapirest = await simsacoreService.GetAllMarcas();

        //    List<Marca> listademarcas = new List<Marca>();

        //    foreach (var item in resultadoapirest.ListadeMrcas)
        //    {
        //        Marca marca = new Marca();
        //        marca.Idmarca = item.Idmarca;
        //        marca.Idempresa = item.Idempresa;
        //        marca.Img = item.Img;
        //        marca.Marca1 = item.Marca1;        
        //        marca.Idtipo = item.Idtipo;
        //        marca.Idestatus = item.Idestatus;
        //        marca.Fchcreacion = item.Fchcreacion;
        //        marca.Fchmod = item.Fchmod;
        //        marca.Fchbaja = item.Fchbaja;
        //        marca.Idusuario = item.Idusuario;

        //        var existe = await context.Marcas.AnyAsync(x => x.Idmarca == item.Idmarca);
        //        if (!existe)
        //        {
        //            listademarcas.Add(marca);
        //        }
        //    }

        //    context.Marcas.AddRange(listademarcas);
        //    await context.SaveChangesAsync(user.Id);
        //    return Ok();
        //}

        //[HttpPost]
        //public async Task<ActionResult> Post(Departamento depto1)
        //{
        //    var user = await _userManager.GetUserAsync(HttpContext.User);

        //    var resultadoapirest = await simsacoreService.GetAllDepartamentos();

        //    List<Departamento> listadedepartamentos = new List<Departamento>();

        //    foreach (var item in resultadoapirest.ListadeDepartamentos)
        //    {
        //        Departamento depto = new Departamento();
        //        depto.Iddepartamento = item.Iddepartamento;
        //        depto.Idempresa = item.Idempresa;
        //        depto.Departamento1 = item.Departamento1;               
        //        depto.Fchcreacion = item.Fchcreacion;
        //        depto.Fchmod = item.Fchmod;
        //        depto.Fchbaja = item.Fchbaja;
        //        depto.Idusuario = item.Idusuario;
        //        //depto.Idestatus = item.Idestatus;

        //        var existe = await context.Departamentos.AnyAsync(x => x.Iddepartamento == item.Iddepartamento);
        //        if (!existe)
        //        {
        //            listadedepartamentos.Add(depto);
        //        }
        //    }

        //    context.Departamentos.AddRange(listadedepartamentos);
        //    await context.SaveChangesAsync(user.Id);
        //    return Ok();
        //}

        //[HttpPost]
        //public async Task<ActionResult> Post(Area area1)
        //{
        //    var user = await _userManager.GetUserAsync(HttpContext.User);

        //    var resultadoapirest = await simsacoreService.GetAllAreas();

        //    List<Area> listadeareas = new List<Area>();

        //    foreach (var item in resultadoapirest.ListadeAreas)
        //    {
        //        Area area = new Area();
        //        area.Idarea = item.Idarea;
        //        area.Iddepartamento = item.Iddepartamento;
        //        area.Area1 = item.Area1;

        //        #region ConvertirStringenDatetime
        //        ////verificar si la fecha no viene null
        //        //if (!String.IsNullOrEmpty(item.Fchcreacion))
        //        //{          
        //        //    //reemplazar el guin por un slash
        //        //    var fechacreacion = item.Fchcreacion.Replace(@"-", "/");
        //        //    //convertir string en fecha
        //        //    area.Fchcreacion = DateTime.ParseExact(fechacreacion, "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);                   
        //        //}
        //        //if(!String.IsNullOrEmpty(item.Fchmod))
        //        //{
        //        //    var fechamod = item.Fchmod.Replace(@"-", "/");
        //        //    area.Fchmod = DateTime.ParseExact(fechamod, "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);                    
        //        //}
        //        //if(!String.IsNullOrEmpty(item.Fchbaja))
        //        //{
        //        //    var fechabaja = item.Fchbaja.Replace(@"-", "/");
        //        //   area.Fchbaja = DateTime.ParseExact(fechabaja, "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);                    
        //        //}
        //        #endregion

        //        area.Fchcreacion = item.Fchcreacion;
        //        area.Fchmod = item.Fchmod;
        //        area.Fchbaja = item.Fchbaja;

        //        area.Idusuario = item.Idusuario;               

        //        var existe = await context.Areas.AnyAsync(x => x.Idarea == item.Idarea);
        //        if (!existe)
        //        {
        //            listadeareas.Add(area);
        //        }
        //    }

        //    context.Areas.AddRange(listadeareas);
        //    await context.SaveChangesAsync(user.Id);
        //    return Ok();
        //}

        //[HttpPost]
        //public async Task<ActionResult> Post(Puesto puesto1)
        //{
        //    var user = await _userManager.GetUserAsync(HttpContext.User);

        //    var resultadoapirest = await simsacoreService.GetAllPuestos();

        //    List<Puesto> listadepuestos = new List<Puesto>();

        //    foreach (var item in resultadoapirest.ListadePuestos)
        //    {
        //        Puesto puesto = new Puesto();
        //        puesto.Idpuesto = item.Idpuesto;               
        //        puesto.Puesto1 = item.Puesto1;
        //        puesto.Fchcreacion = item.Fchcreacion;
        //        puesto.Fchmod = item.Fchmod;
        //        puesto.Fchbaja = item.Fchbaja;
        //        puesto.Idusuario = item.Idusuario;

        //        var existe = await context.Puestos.AnyAsync(x => x.Idpuesto == item.Idpuesto);
        //        if (!existe)
        //        {
        //            listadepuestos.Add(puesto);
        //        }
        //    }

        //    context.Puestos.AddRange(listadepuestos);
        //    await context.SaveChangesAsync(user.Id);
        //    return Ok();
        //}

        [Route("ActualizarPersona")]
        [HttpPost]
        public async Task<ActionResult> PostPersona(Persona persona1)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var resultadoapirest = await simsacoreService.GetAllPersonas();

            List<Persona> listadepersonas = new List<Persona>();

            foreach (var item in resultadoapirest.ListadePersonas)
            {
                Persona persona = new Persona();
                persona.Idpersona = item.Idpersona;
                persona.ApePat = item.ApePat;
                persona.ApeMat = item.ApeMat;
                persona.Nombre = item.Nombre;
                persona.Sexo = item.Sexo;

                //si la fecha de nacimiento viene null guarda la fecha en 0000-00-00
                if (item.Fchnac == null)
                {
                    persona.Fchnac = new DateTime(0001, 01, 01); //no se puede guardar 0000-00-00 da error
                }
                else
                {
                    //sino viene null debes separar la fecha en variables por que la bd espera un datetime sin aceptar null y despues crear una nueva fecha que no permita null
                    //la clase donde se deserealiza la api person si permite null,
                    //permite null por que para deserealizar la fecha viene 0000-00-00 y lo toma como string luego ya no puede convertir string en datetime (paso lo mismo en areas)

                    int dia = 0;
                    int mes = 0;
                    int ano = 0;

                    //por cada fecha de nacimientos obtenemos el año
                    int year = Convert.ToInt32(item.Fchnac.Value.Year);

                    //una vez obtenido el año verificamos si el año es biciesto
                    if (year % 400 == 0 || (year % 4 == 0 && year % 100 != 0))
                    {
                        dia = Convert.ToInt32(item.Fchnac.Value.Day);
                        mes = Convert.ToInt32(item.Fchnac.Value.Month);
                        ano = Convert.ToInt32(item.Fchnac.Value.Year);

                        /*si es biciesto nos permite crear una fecha fuera de rango o inexistente como un 29 de febrero
                          sino verificamos que el año es biciesto da un error:Year, Month, and Day parameters describe an un-representable DateTime
                          ese error es por que no puedes crear una fecha que no existe por eso verificamos si el año es biciesto*/
                        var FechaBiciesta = new DateTime(year, mes, dia);
                        persona.Fchnac = FechaBiciesta;
                    }
                    else
                    {
                        dia = Convert.ToInt32(item.Fchnac.Value.Day);
                        mes = Convert.ToInt32(item.Fchnac.Value.Month);
                        ano = Convert.ToInt32(item.Fchnac.Value.Year);

                        var fechasinnull = new DateTime(year, mes, dia);
                        persona.Fchnac = fechasinnull;
                    }
                }

                persona.Rfc = item.Rfc;
                persona.Curp = item.Curp;
                persona.Nss = item.Nss;
                persona.Calle = item.Calle;
                persona.Noext = item.Noext;
                persona.Noint = item.Noint;
                persona.Colonia = item.Colonia;
                persona.Cp = item.Cp;
                persona.Ciudad = item.Ciudad;
                persona.Estado = item.Estado;
                persona.Correo = item.Correo;
                persona.Telefono = item.Telefono;

                var existe = await context.Personas.AnyAsync(x => x.Idpersona == item.Idpersona);
                if (!existe)
                {
                    listadepersonas.Add(persona);
                }
            }

            context.Personas.AddRange(listadepersonas);
            await context.SaveChangesAsync(user.Id);
            return Ok();
        }

        [Route("ActualizarEmpleado")]
        [HttpPost]
        public async Task<ActionResult> PostEmpleado(Empleado empleado1)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var resultadoapirest = await simsacoreService.GetAllEmpleados();

            List<Empleado> listadeempleados = new List<Empleado>();

            foreach (var item in resultadoapirest.ListadeEmpleados)
            {
                Empleado empleado = new Empleado();
                empleado.Idempleado = item.Idempleado;
                empleado.Idpersona = item.Idpersona;
                empleado.Img = item.Img;
                empleado.Noemp = item.Noemp;
                empleado.Correo = item.Correo;
                empleado.Telefono = item.Telefono;

                empleado.Iddepartamento = item.Iddepartamento;
                empleado.Idarea = item.Idarea;
                empleado.Idpuesto = item.Idpuesto;
                empleado.Idpagadora = item.Idpagadora;
                empleado.ZonaId = item.ZonaId;
                empleado.Idestacion = item.Idestacion;

                empleado.TiendaId = item.TiendaId;
                empleado.Nomina = item.Nomina;
                empleado.Variable = item.Variable;
                empleado.Idtipo = item.Idtipo;
                empleado.Idestatus = item.Idestatus;
                empleado.Fchalta = item.Fchalta;

                empleado.Fchactualizado = item.Fchactualizado;
                empleado.Actualizado_por = item.Actualizado_por;
                empleado.Fchbaja = item.Fchbaja;
                empleado.Borrado_por = item.Borrado_por;

                var existe = await context.Empleados.AnyAsync(x => x.Idempleado == item.Idempleado);
                if (!existe)
                {
                    listadeempleados.Add(empleado);
                }
            }

            context.Empleados.AddRange(listadeempleados);
            await context.SaveChangesAsync(user.Id);
            return Ok();
        }

        [Route("ActualizarEstacion")]
        [HttpPost]
        public async Task<ActionResult> PostEstacion(Estacion estacion1)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var resultadoapirest = await simsacoreService.GetAllEstaciones();

            List<Estacion> listadeestaciones = new List<Estacion>();

            foreach (var item in resultadoapirest.ListadeEstaciones)
            {
                Estacion estacion = new Estacion();
                estacion.Idestacion = item.Idestacion;
                estacion.Codgas = item.Codgas;
                estacion.Nomcg = item.Nomcg;
                estacion.Img = item.Img;
                estacion.Permisocre = item.Permisocre;
                estacion.Noest = item.Noest;

                estacion.Nombre = item.Nombre;
                estacion.Marca = item.Marca;
                estacion.Correo = item.Correo;
                estacion.Tel = item.Tel;
                estacion.Url = item.Url;
                estacion.Idestacion = item.Idestacion;

                estacion.Razonsocial = item.Razonsocial;
                estacion.Calle = item.Calle;
                estacion.Noint = item.Noint;
                estacion.Noext = item.Noext;
                estacion.Colonia = item.Colonia;
                estacion.Municipio = item.Municipio;

                estacion.Estado = item.Estado;
                estacion.Cp = item.Cp;
                estacion.Zona = item.Zona;
                estacion.Latitud = item.Latitud;

                estacion.Longitud = item.Longitud;
                estacion.Qr = item.Qr;
                estacion.Fchcreacion = item.Fchcreacion;
                estacion.Fchmodificacion = item.Fchmodificacion;

                estacion.Idusuario = item.Idusuario;
                estacion.Estatus = item.Estatus;
                estacion.Idrazonsocial = item.Idrazonsocial;

                var existe = await context.Estaciones.AnyAsync(x => x.Idestacion == item.Idestacion);
                if (!existe)
                {
                    listadeestaciones.Add(estacion);
                }
            }

            context.Estaciones.AddRange(listadeestaciones);
            await context.SaveChangesAsync(user.Id);
            return Ok();
        }
    }
}
