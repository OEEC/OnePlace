﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnePlace.Server.Data;
using OnePlace.Shared.DTOs;
using OnePlace.Shared.Entidades.SimsaCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OnePlace.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrador,Usuario")]
    public class BirthdayController : ControllerBase
    {
        private readonly oneplaceContext context;
        private readonly UserManager<ApplicationUser> _userManager;
        public BirthdayController(oneplaceContext context, UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            _userManager = userManager;
        }

        [Route("listadecumpleaneros")]
        [HttpGet]
        public async Task<ActionResult<List<EmpleadoPersonaDTO>>> GetListado()
        {
            //traemos el listado de empleados
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
                                            Persona = context.Personas.Where(x => x.Idpersona == e.Idpersona).FirstOrDefault(),
                                            Departamento = context.Departamentos.Where(x => x.Iddepartamento == e.Iddepartamento).FirstOrDefault(),
                                            Area = context.Areas.Where(x => x.Idarea == e.Idarea).FirstOrDefault(),
                                            Puesto = context.Puestos.Where(x => x.Idpuesto == e.Idpuesto).FirstOrDefault()
                                        }).ToList();

            List<EmpleadoPersonaDTO> listadoempleadosconcumple = new List<EmpleadoPersonaDTO>();

            //fijar una fecha en este caso una fecha de inicio
            DateTime FechaActual = DateTime.Now;
            DateTime FechaInicio = new DateTime(FechaActual.Year, FechaActual.Month, 1);

            //obtener el total de dias de un mes en este caso el actual
            int DiaFinMes = DateTime.DaysInMonth(FechaActual.Year, FechaActual.Month);
            DateTime FechaFinal = new DateTime(FechaActual.Year, FechaActual.Month, DiaFinMes);

            //recorremos el listado de empleados para extraer su fecha de nacimiento 
            foreach (var item in empleados)
            {
                //separamos la fecha de nacimiento en variables de tipo entero para mandarlas al metodo que obtendra el proximo cumpleaños
                string fechadenacimiento = item.Persona.Fchnac.ToString();
                DateTime myDateTime = DateTime.Parse(fechadenacimiento);
                int dia = Convert.ToInt32(myDateTime.Day);
                int mes = Convert.ToInt32(myDateTime.Month);
                int ano = Convert.ToInt32(myDateTime.Year);

                if (string.IsNullOrEmpty(item.Img))
                {
                    if (item.Persona.Sexo == "M")
                    {
                        // Aquí colocas la URL de la imagen por defecto
                        item.Img = "Img" + "/" + "hombre3d.jpg";
                    }
                    else
                    {
                        item.Img = "Img" + "/" + "mujer3d.jpg";
                    }
                }

                var model = new EmpleadoPersonaDTO();
                model.Empleado = item;
                //model.Persona = persona;
                //almacenamos el resultado del metodo proximocumpleaños en el dto
                model.ProximoCumpleTodoMes = ProximoCumpleTodoMes(dia, mes, ano);

                //una vez que tenemos el dato del empleado mas su fecha de proximo cumple en el dto la comparamos,
                //si esta entre los dias de inicio y fin de mes lo guardamos en la lista de dto´s
                if (model.ProximoCumpleTodoMes >= FechaInicio && model.ProximoCumpleTodoMes <= FechaFinal)
                {
                    listadoempleadosconcumple.Add(model);
                }
            }

            return listadoempleadosconcumple;
        }

        [HttpGet]
        public async Task<ActionResult<EmpleadoPersonaDTO>> Get()
        {
            //obtenemos el usuario logueado
            var user = await _userManager.GetUserAsync(HttpContext.User);

            //buscamos el empleado por medio del usuario logueado
            var empleado = await context.Empleados.Where(x => x.Idempleado == user.Idempleado).FirstOrDefaultAsync();

            //buscamos la persona por medio del empleado que pertenece al usuario logueado
            var persona = await context.Personas.Where(x => x.Idpersona == empleado.Idpersona).FirstOrDefaultAsync();

            //separamos la fecha de nacimiento en variables de tipo entero para mandarlas al metodo que obtendra el proximo cumpleaños
            string fechadenacimiento = persona.Fchnac.ToString();
            DateTime myDateTime = DateTime.Parse(fechadenacimiento);
            int dia = Convert.ToInt32(myDateTime.Day);
            int mes = Convert.ToInt32(myDateTime.Month);
            int ano = Convert.ToInt32(myDateTime.Year);

            var model = new EmpleadoPersonaDTO();
            model.Empleado = empleado;
            model.Persona = persona;

            //almacenamos el resultado del metodo proximocumpleaños en el dto
            model.ProximoCumple = ProximoCumple(dia, mes, ano);

            return model;
        }
        public string ProximoCumple(int diaCumple, int mesCumple, int anioCumple)
        {
            //int diaCumple = 4;//Dia del Cumpleanios
            //int mesCumple = 4;//Mes de Cumple 4=Abril
            //int anioCumple = 1984; //Anio de Cumple
            //DateTime fechaNacimiento = new DateTime(anioCumple, mesCumple, diaCumple);

            ////Se calcula la Edad Actual A partir de la fecha actual Sustrayendo la fecha de nacimiento
            ////esto devuelve un TimeSpan por tanto tomaremos los Dias y lo dividimos en 365 días
            //int edad = (DateTime.Now.Subtract(fechaNacimiento).Days / 365);

            DateTime proximoCumple;
            //Define el proximo Cumple,
            //En caso de que el mes de cumple, sea menor al Mes Actual, se busca el Próxima fecha que seria del año que viene
            //es por ello el AddYear(1)
            //En caso de ser mayor se toma el año actual
            if (mesCumple < DateTime.Now.Month)
            {
                proximoCumple = new DateTime(DateTime.Now.AddYears(1).Year, mesCumple, diaCumple);
            }
            else
            {
                proximoCumple = new DateTime(DateTime.Now.Year, mesCumple, diaCumple);
            }

            //Definiremos los dias faltantes para el proximo cumple
            TimeSpan faltan = proximoCumple.Subtract(DateTime.Today);

            //los dias salian en negativo por eso los iba a transformar en positivo
            //var negativoApositivo = Math.Abs(faltan.Days);

            var fechaARetornar = DateTime.Today.AddDays(faltan.Days);

            //Ahora Elaboramos el Mensaje
            //StringBuilder sb = new StringBuilder();
            //sb.AppendFormat("Usted Tiene {0} Años ", edad);
            //sb.AppendFormat("y tu Próximo Cumpleaños es: {0} Días", faltan.Days);
            //sb.AppendFormat(", {0} Horas ", faltan.Hours);
            //sb.AppendFormat("y {0} Minutos ", faltan.Minutes);           

            //retornamos la fecha en formato string sin horas,minutos para no tener problemas con ello, mientras sea el dia correcto
            return fechaARetornar.ToString("dd/MM/yyyy");
        }
        public DateTime ProximoCumpleTodoMes(int diaCumple, int mesCumple, int anioCumple)
        {
            DateTime proximoCumple;
            //Define el proximo Cumple,
            //En caso de que el mes de cumple, sea menor al Mes Actual, se busca el Próxima fecha que seria del año que viene
            //es por ello el AddYear(1)
            //En caso de ser mayor se toma el año actual
            if (mesCumple < DateTime.Now.Month)
            {
                proximoCumple = new DateTime(DateTime.Now.AddYears(1).Year, mesCumple, diaCumple);
            }
            else
            {
                proximoCumple = new DateTime(DateTime.Now.Year, mesCumple, diaCumple);
            }

            //Definiremos los dias faltantes para el proximo cumple
            TimeSpan faltan = proximoCumple.Subtract(DateTime.Today);

            var fechaARetornar = DateTime.Today.AddDays(faltan.Days);

            return fechaARetornar;
        }
    }
}