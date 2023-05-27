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
		public async Task<ActionResult<PaginadorGenerico<EmpleadoPersonaDTO>>> GetListado([FromQuery] ParametrosBusquedaEquiposReparacion parametrosBusqueda)
		{
			PaginadorGenerico<EmpleadoPersonaDTO> _PaginadorConceptos;

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
											ImagenesCumple = context.ImagenesCumpleEmpleado.Where(x => x.EmpleadoId == e.Idempleado).ToList(),
											Persona = context.Personas.Where(x => x.Idpersona == e.Idpersona).FirstOrDefault(),
											Departamento = context.Departamentos.Where(x => x.Iddepartamento == e.Iddepartamento).FirstOrDefault(),
											Area = context.Areas.Where(x => x.Idarea == e.Idarea).FirstOrDefault(),
											Puesto = context.Puestos.Where(x => x.Idpuesto == e.Idpuesto).FirstOrDefault()
										}).ToList();

			List<EmpleadoPersonaDTO> listadoempleadosconcumple = new List<EmpleadoPersonaDTO>();
			List<EmpleadoPersonaDTO> listabuffer = new List<EmpleadoPersonaDTO>();

			//fijar una fecha en este caso una fecha de inicio
			DateTime FechaActual = DateTime.Now;
			DateTime FechaInicio = new DateTime(FechaActual.Year, FechaActual.Month, 1);

			//obtener el total de dias de un mes en este caso el actual
			int DiaFinMes = DateTime.DaysInMonth(FechaActual.Year, FechaActual.Month);
			DateTime FechaFinal = new DateTime(FechaActual.Year, FechaActual.Month, DiaFinMes);

			//recorremos el listado de empleados para extraer su fecha de nacimiento 
			foreach (var item in empleados)
			{
				if (string.IsNullOrEmpty(item.Img))
				{
					if (item.Persona != null)
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
				}

				var model = new EmpleadoPersonaDTO();
				model.Empleado = item;
				//model.Persona = persona; //ya viene incluido en la data del empleado por eso ya no lo ponemos lo mandamos null               

				//variables
				int dia = 0;
				int mes = 0;
				int ano = 0;

				if (item.Persona != null)
				{
					//por cada fecha de nacimientos obtenemos el año
					int year = Convert.ToInt32(item.Persona != null ? item.Persona.Fchnac?.Year : DateTime.MinValue.Year);

					//una vez obtenido el año verificamos si el año es biciesto
					if (year % 400 == 0 || (year % 4 == 0 && year % 100 != 0))
					{
						/*si es biciesto nos permite crear una fecha fuera de rango o inexistente como un 29 de febrero
                        sino verificamos que el año es biciesto da un error:Year, Month, and Day parameters describe an un-representable DateTime
                        ese error es por que no puedes crear una fecha que no existe por eso verificamos si el año es biciesto*/
						var FechaBiciesta = new DateTime(year, 2, 29);//creamos fecha del 29 de febrero

						//si la fecha de nacimientos es un 29 de febrero cambiamos el 29 por 28 para que celebren su cumpleaños cada año y no cada 4
						//ademas para que el valor sea permitido en el metodo ProximoCumple.ProximoCumpleTodoMes
						if (item.Persona.Fchnac == FechaBiciesta)
						{
							//separamos la fecha de nacimiento en variables de tipo entero para mandarlas al metodo que obtendra el proximo cumpleaños
							dia = 28;
							mes = Convert.ToInt32(item.Persona.Fchnac?.Month);
							ano = Convert.ToInt32(item.Persona.Fchnac?.Year);
							//almacenamos el resultado del metodo proximocumpleaños en el dto
							model.ProximoCumpleTodoMes = ProximoCumple.ProximoCumpleTodoMes(dia, mes, ano);
						}
					}
					else
					{
						//si el año no es biciesto pasamos el dia original
						dia = Convert.ToInt32(item.Persona.Fchnac?.Day);
						mes = Convert.ToInt32(item.Persona.Fchnac?.Month);
						ano = Convert.ToInt32(item.Persona.Fchnac?.Year);
						model.ProximoCumpleTodoMes = ProximoCumple.ProximoCumpleTodoMes(dia, mes, ano);
					}
				}

				//una vez que tenemos el dato del empleado mas su fecha de proximo cumple en el dto la comparamos,
				//si esta entre los dias de inicio y fin de mes lo guardamos en la lista de dto´s
				if (model.ProximoCumpleTodoMes >= FechaInicio && model.ProximoCumpleTodoMes <= FechaFinal)
				{
					listadoempleadosconcumple.Add(model);
				}
			}

			//buffer con el total de cumpleañeros sin paginacion
			listabuffer = listadoempleadosconcumple;

			var queryable = listadoempleadosconcumple.AsQueryable();

			#region PAGINACION          

			int _TotalRegistros = 0;
			int _TotalPaginas = 0;

			// Número total de registros de la coleccion 
			_TotalRegistros = queryable.Count();

			// Obtenemos la 'página de registros' de la coleccion 
			queryable = queryable.Skip((parametrosBusqueda.Pagina - 1) * parametrosBusqueda.CantidadRegistros)
											   .Take(parametrosBusqueda.CantidadRegistros);

			// Número total de páginas de la coleccion
			_TotalPaginas = (int)Math.Ceiling((double)_TotalRegistros / parametrosBusqueda.CantidadRegistros);

			//Instanciamos la 'Clase de paginación' y asignamos los nuevos valores
			_PaginadorConceptos = new PaginadorGenerico<EmpleadoPersonaDTO>()
			{
				RegistrosPorPagina = parametrosBusqueda.CantidadRegistros,
				TotalRegistros = _TotalRegistros,
				TotalPaginas = _TotalPaginas,
				PaginaActual = parametrosBusqueda.Pagina,
				Resultado = queryable.ToList(),//filtrado por categoria mas paginacion
				ResultadoAExportar = listabuffer
			};
			return _PaginadorConceptos;

			#endregion
		}
		public class ParametrosBusquedaEquiposReparacion
		{
			public int Pagina { get; set; } = 1;
			public int CantidadRegistros { get; set; } = 10;
			public PaginacionDTO Paginacion
			{
				get { return new PaginacionDTO() { Pagina = Pagina, CantidadRegistros = CantidadRegistros }; }
			}
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

			var model = new EmpleadoPersonaDTO();
			model.Empleado = empleado;
			model.Persona = persona;

			//variables
			int dia = 0;
			int mes = 0;
			int ano = 0;

			//por cada fecha de nacimientos obtenemos el año
			int year = Convert.ToInt32(persona.Fchnac?.Year);

			//una vez obtenido el año verificamos si el año es biciesto
			if (year % 400 == 0 || (year % 4 == 0 && year % 100 != 0))
			{
				/*si es biciesto nos permite crear una fecha fuera de rango o inexistente como un 29 de febrero
                sino verificamos que el año es biciesto da un error:Year, Month, and Day parameters describe an un-representable DateTime
                ese error es por que no puedes crear una fecha que no existe por eso verificamos si el año es biciesto*/
				var FechaBiciesta = new DateTime(year, 2, 29);//creamos fecha del 29 de febrero

				//si la fecha de nacimientos es un 29 de febrero cambiamos el 29 por 28 para que celebren su cumpleaños cada año y no cada 4
				//ademas para que el valor sea permitido en el metodo ProximoCumple.ProximoCumpleTodoMes
				if (persona.Fchnac == FechaBiciesta)
				{
					//separamos la fecha de nacimiento en variables de tipo entero para mandarlas al metodo que obtendra el proximo cumpleaños
					dia = 28;
					mes = Convert.ToInt32(persona.Fchnac?.Month);
					ano = Convert.ToInt32(persona.Fchnac?.Year);
					//almacenamos el resultado del metodo proximocumpleaños en el dto
					model.ProximoCumple = ProximoCumple.ProximoCumpleaños(dia, mes, ano);
				}
			}
			else
			{
				//si el año no es biciesto pasamos el dia original
				dia = Convert.ToInt32(persona.Fchnac?.Day);
				mes = Convert.ToInt32(persona.Fchnac?.Month);
				ano = Convert.ToInt32(persona.Fchnac?.Year);
				model.ProximoCumple = ProximoCumple.ProximoCumpleaños(dia, mes, ano);
			}

			//almacenamos el codigo qr 
			var codigo = "Regalo" + model.Empleado.Noemp;
			model.CodigoQR = GenerarQR.GenerarCode(codigo);

			return model;
		}

		[HttpPost]
		public async Task<ActionResult<int>> Post(EstadodeCumpleaños estado)
		{
			var user = await _userManager.GetUserAsync(HttpContext.User);
			estado.UserId = user.Id;

			var empleado = await context.Empleados.Where(x => x.Idempleado == user.Idempleado).FirstOrDefaultAsync();
			estado.Idempleado = empleado.Idempleado;

			context.Add(estado);
			await context.SaveChangesAsync(user.Id);
			return estado.EstadodeCumpleañosId;
		}

		[Route("EstadoCumple")]
		[HttpGet]
		public async Task<ActionResult<EstadodeCumpleaños>> GetEstadoCumple()
		{
			var user = await _userManager.GetUserAsync(HttpContext.User);

			var estado = await context.EstadodeCumpleaños.Where(x => x.UserId == user.Id).FirstOrDefaultAsync();
			if (estado == null) { return NotFound(); }
			return estado;
		}
	}
}