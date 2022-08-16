using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnePlace.Server.Data;
using OnePlace.Server.Helpers;
using OnePlace.Server.Reportes;
using OnePlace.Shared.DTOs;
using OnePlace.Shared.Entidades.Reporteador;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnePlace.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrador,Usuario")]
    public class TicketController : ControllerBase
    {       
        private readonly oneplaceContext context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper mapper;

        public TicketController(oneplaceContext context, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            this.context = context;
            _userManager = userManager;
            this.mapper = mapper;
        }

        [HttpGet("iframe/{id}/{tipo}")]
        [AllowAnonymous]
        public IActionResult Get(int id, TipoReporte tipo)
        {
            var bytes16 = new Byte[16];            
            byte[] salida = new byte[0];

            switch (tipo)
            {
                case TipoReporte.TarjetadeCumple:
                                        
                    //buscamos el empleado por medio del usuario logueado
                    var empleado = context.Empleados.Where(x => x.Idempleado == id).FirstOrDefault();

                    //buscamos la persona por medio del empleado que pertenece al usuario logueado
                    var persona = context.Personas.Where(x => x.Idpersona == empleado.Idpersona).FirstOrDefault();

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
                    model.ProximoCumple = ProximoCumple.ProximoCumpleaños(dia, mes, ano);

                    //almacenamos el codigo qr 
                    var codigo = "Regalo" + model.Empleado.Noemp;
                    model.CodigoQR = GenerarQR.GenerarCode(codigo);

                    salida = GenerarTickets.TarjetadeCumple(model);

                    //se comento esta linea por que ya no retornamos un file pdf para verlo,sino solo los bytes para descargarlo
                    //return File(salida, "application/pdf");
                    return Ok(salida);

                default:
                    return NotFound("Este caso no esta en el switch");
            }
        }
    }
}
