using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnePlace.Client.Service;
using OnePlace.Server.Data;
using OnePlace.Shared.Entidades.SimsaCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnePlace.Server.Services
{
    public interface IApiEmpleadosService
    {
        Task DatosdeApiABaseDatosEmpleados();
    }
    public class ApiEmpleadosService : IApiEmpleadosService
    {
        private readonly oneplaceContext context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ILogger<ApiaBdService> logger;
        private readonly ISimsacoreService simsacoreService;
        public ApiEmpleadosService(oneplaceContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor, ILogger<ApiaBdService> logger, ISimsacoreService simsacoreService)
        {
            this.context = context;
            this.logger = logger;
            _userManager = userManager;
            this.httpContextAccessor = httpContextAccessor;
            this.simsacoreService = simsacoreService;
        }
        public async Task DatosdeApiABaseDatosEmpleados()
        {
            logger.LogInformation("Se ejecuto un job para obtener datos de un api y guardarlos en la bd");            

            var resultadopersonas = await simsacoreService.GetAllPersonas();

            List<Persona> listadepersonas = new List<Persona>();

            foreach (var item in resultadopersonas.ListadePersonas)
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
            await context.SaveChangesAsync();

            var resultadoempleados = await simsacoreService.GetAllEmpleados();

            List<Empleado> listadeempleados = new List<Empleado>();

            foreach (var item in resultadoempleados.ListadeEmpleados)
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
            await context.SaveChangesAsync();            
        }
    }
}
