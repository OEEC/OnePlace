using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnePlace.Server.Data;
using OnePlace.Shared.Entidades;
using OnePlace.Shared.Entidades.SimsaCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using OnePlace.Shared.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnePlace.Shared.IdentityModels;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using OnePlace.Shared.DTOs.Modelos;

namespace OnePlace.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QRespuestaController : ControllerBase
    {
        private readonly oneplaceContext context;
        private readonly UserManager<IdentityUsuario> userManager;
        private readonly IMapper mapper;

        public QRespuestaController(oneplaceContext context, UserManager<IdentityUsuario> userManager, IMapper mapper)
        {
            this.context = context;
            this.userManager = userManager;
            this.mapper = mapper;
        }

        [HttpPost("guardar")]
        public async Task<IActionResult> GuardarRespuestas([FromBody] List<QRespuestaDTO> respuestasDto)
        {
            var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            if (user == null)
            {
                return Unauthorized("Usuario no autenticado.");
            }

            try
            {
                if (respuestasDto != null || respuestasDto.Any())
                {
                    // Iterar sobre las respuestas enviadas
                    foreach (var dto in respuestasDto)
                    {
                        // Verificar si ya existe una respuesta para el usuario y la pregunta
                        var respuestaExistente = await context.QRespuesta
                            .FirstOrDefaultAsync(r => r.PreguntaId == dto.PreguntaId && r.UsuarioId == user.Id && r.IdRespuesta != dto.IdRespuesta);

                        if (respuestaExistente != null)
                        {
                            // Actualizar la respuesta existente
                            respuestaExistente.Respuesta = dto.Respuesta;
                            respuestaExistente.fecha = dto.Fecha ?? DateTime.Now; // Actualizar la fecha si es necesario
                        }
                        else
                        {
                            // Crear una nueva respuesta si no existe
                            var nuevaRespuesta = new QRespuesta
                            {
                                PreguntaId = dto.PreguntaId,
                                UsuarioId = user.Id,
                                Respuesta = dto.Respuesta,
                                fecha = dto.Fecha ?? DateTime.Now
                            };

                            context.QRespuesta.Add(nuevaRespuesta);
                        }
                    }

                    // Guardar los cambios en la base de datos
                    await context.SaveChangesAsync();
                    return Ok("Respuestas guardadas/actualizadas exitosamente.");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al guardar respuestas: {ex.Message}");
            }
        }

        [HttpPost("guardarRecom")]
        public async Task<IActionResult> GuardarRecomendaciones([FromBody] List<QRespuestaDTO> respuestasDto)
        {
            var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            if (user == null)
            {
                return Unauthorized("Usuario no autenticado.");
            }

            try
            {
                if (respuestasDto == null || !respuestasDto.Any())
                {
                    return BadRequest("No hay respuestas para guardar.");
                }

                foreach (var dto in respuestasDto)
                {
                    // Si el IdRespuesta viene con valor, intentamos actualizar esa respuesta
                    QRespuesta respuestaExistente = null;
                    if (dto.IdRespuesta.HasValue && dto.IdRespuesta.Value > 0)
                    {
                        respuestaExistente = await context.QRespuesta
                            .FirstOrDefaultAsync(r => r.PreguntaId == dto.PreguntaId
                                                   && r.UsuarioId == user.Id
                                                   && r.IdRespuesta == dto.IdRespuesta.Value);
                    }

                    if (respuestaExistente != null)
                    {
                        // Actualizamos la respuesta existente
                        respuestaExistente.Respuesta = dto.Respuesta;
                        respuestaExistente.fecha = dto.Fecha ?? DateTime.Now;
                    }
                    else
                    {
                        // Creamos una nueva respuesta
                        var nuevaRespuesta = new QRespuesta
                        {
                            PreguntaId = dto.PreguntaId,
                            UsuarioId = user.Id,
                            Respuesta = dto.Respuesta,
                            fecha = dto.Fecha ?? DateTime.Now
                        };

                        context.QRespuesta.Add(nuevaRespuesta);
                    }
                }

                await context.SaveChangesAsync();
                return Ok("Respuestas guardadas/actualizadas exitosamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al guardar respuestas: {ex.Message}");
            }
        }

        [HttpGet("grupo/usuario/{grupoId}")]
        public async Task<IActionResult> ObtenerRespuestasPorGrupo(int grupoId)
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                // Obtener las respuestas del usuario para las preguntas del grupo
                var respuestas = await context.QRespuesta
                    .Where(r => r.UsuarioId == user.Id && r.Pregunta.GrupoId == grupoId)
                    .Select(r => new QRespuestaDTO
                    {
                        PreguntaId = r.PreguntaId,
                        UsuarioId = r.UsuarioId,
                        Respuesta = r.Respuesta,
                        Fecha = r.fecha
                    })
                    .ToListAsync();

                return Ok(respuestas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener respuestas: {ex.Message}");
            }

        }

        [HttpGet("encargados")]
        public async Task<IActionResult> ObtenerEncargadosDeEmpleado()
        {
            try
            {
                int supervisorId = 0;
                int jefeId = 0;
                var supervisorEstacionId = 40;
                var supervisorTiendaId = 77;
                var jefeTiendaPuestoId = 214;
                var jefeDeTurnoPuestoId = 32;
                var gerentePuestoIds = new[] { 23, 24, 25, 26, 27, 28, 39, 49, 50, 139, 140, 141,
                                               142, 143, 144, 145, 146, 147, 148, 149, 150, 151,
                                                208, 209, 210, 211, 212, 213, 219 };

                EncargadosDTO encargados = new();

                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                if (user is null) return BadRequest();

                // Cargar el empleado actual
                var empleado = await context.Empleados.Where(e => e.Idempleado == user.Idempleado)
                    .Include(x => x.Puesto)
                    .Include(x => x.Estacion)
                    .Select(x => mapper.Map<EmpleadoDTO>(x)).FirstOrDefaultAsync();
                if (empleado is null) return BadRequest();

                var relacionencargadosestaciones = await context.EmpleadoEstaciones
                    .Where(x => x.EstacionId == empleado.Estacion.Id)
                    .Include(x => x.Empleado.Persona)
                    .ToListAsync();

                if (empleado.Division == "TIENDAS")
                {
                    supervisorId = 77;
                    jefeId = 214;
                }
                else if (empleado.Division == "ESTACIONES")
                {
                    supervisorId = 40;
                    jefeId = 32;
                }

                if (empleado.Puesto.Id == supervisorEstacionId || empleado.Puesto.Id == supervisorTiendaId)
                {
                    encargados.Supervisor = context.Empleados
                        .Where(x => x.Noemp == "0000001")
                        .Include(x => x.Persona)
                        .Select(mapper.Map<EmpleadoDTO>)
                        .FirstOrDefault();
                }
                else if (gerentePuestoIds.Contains(empleado.Puesto.Id))
                {
                    //if (empleado.Estacion.Zona == 5)
                    //{
                    //    encargados.Supervisor = context.Empleados
                    //    .Where(x => x.Noemp == "0006868")
                    //    .Include(x => x.Persona)
                    //    .Select(mapper.Map<EmpleadoDTO>)
                    //    .FirstOrDefault();
                    //}
                    //else
                    //{
                        encargados.Supervisor = relacionencargadosestaciones.Where(x => !x.Esgerente && !x.Esjefeturno && x.PuestoId == supervisorId)
                                                                            .Select(x => mapper.Map<EmpleadoDTO>(x.Empleado))
                                                                            .FirstOrDefault();
                    //}
                    encargados.Gerente = encargados.Supervisor;
                }
                else if (empleado.Puesto.Id == jefeDeTurnoPuestoId || jefeTiendaPuestoId == empleado.Puesto.Id)
                {
                    encargados.Supervisor = relacionencargadosestaciones.Where(x => !x.Esgerente && !x.Esjefeturno && x.PuestoId == supervisorId)
                                                                        .Select(x => mapper.Map<EmpleadoDTO>(x.Empleado))
                                                                        .FirstOrDefault();

                    encargados.Gerente = relacionencargadosestaciones.Where(x => x.Esgerente && gerentePuestoIds.Contains(x.PuestoId))
                                                                         .Select(x => mapper.Map<EmpleadoDTO>(x.Empleado))
                                                                         .FirstOrDefault();

                    if (await context.EmpleadoEstaciones.AnyAsync(x => x.EmpleadoId == empleado.Idempleado && x.Esgerente))
                        encargados.Gerente = encargados.Supervisor;

                }
                else
                {
                    encargados.Supervisor = relacionencargadosestaciones.Where(x => !x.Esgerente && !x.Esjefeturno && x.PuestoId == supervisorId)
                                                                        .Select(x => mapper.Map<EmpleadoDTO>(x.Empleado))
                                                                        .FirstOrDefault();
                    encargados.JefeTurno = relacionencargadosestaciones.Where(x => x.Esjefeturno && x.PuestoId == jefeId)
                                                                       .Select(x => mapper.Map<EmpleadoDTO>(x.Empleado))
                                                                       .FirstOrDefault();
                    encargados.Gerente = relacionencargadosestaciones.Where(x => x.Esgerente && gerentePuestoIds.Contains(x.PuestoId))
                                                                     .Select(x => mapper.Map<EmpleadoDTO>(x.Empleado))
                                                                     .FirstOrDefault();
                }

                return Ok(encargados);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error interno del servidor: {ex.Message}");
            }
        }

        public async Task<IActionResult> Encargados()
        {
            //var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            //if (user == null)
            //{
            //    return Unauthorized("Usuario no autenticado.");
            //}

            try
            {
                List<Empleado> encargados = new();
                // IDs de puestos según tu lógica
                var supervisorPuestoId = 77;
                var jefeTiendaPuestoId = 214;
                var jefeDeTurnoPuestoId = 32;
                var gerentePuestoIds = new[] { 23, 24, 25, 26, 27, 28, 39, 49, 50, 139, 140, 141,
                                               142, 143, 144, 145, 146, 147, 148, 149, 150, 151,
                                                208, 209, 210, 211, 212, 213, 219 };

                // Obtener el usuario actual
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);

                // Cargar el empleado actual junto con sus estaciones
                var empleado = await context.Empleados
                    .Include(e => e.Estaciones) // Carga las estaciones asociadas al empleado
                    .FirstOrDefaultAsync(e => e.Idempleado == user.Idempleado);

                if (empleado == null)
                {
                    // Manejo del caso donde no se encuentra el empleado
                    return BadRequest();
                }

                // Si el empleado es supervisor o jefe de turno
                if (empleado.Idpuesto == supervisorPuestoId || empleado.Idpuesto == jefeDeTurnoPuestoId)
                {

                    // Buscar gerentes que estén asignados al menos a una de las mismas estaciones
                    var gerentesRelacionados = await context.Empleados
                        .Include(e => e.Estaciones) // Incluir estaciones para hacer el filtrado
                        .Include(e => e.Persona)
                        .Where(e => e.Idpuesto.HasValue && gerentePuestoIds.Contains(e.Idpuesto.Value))
                        .Where(e => e.Estaciones.Any(est => est.Idestacion == empleado.Idestacion))
                        .ToListAsync();

                    encargados = gerentesRelacionados;
                }
                else if (empleado.Division == "TIENDAS")
                {

                    // Buscar gerentes que estén asignados al menos a una de las mismas estaciones
                    var supervisoresRelacionados = await context.Empleados
                        .Include(e => e.Estaciones) // Incluir estaciones para filtrar
                        .Include(e => e.Persona)
                        .Where(e => e.Idpuesto.HasValue && e.Idpuesto.Value == supervisorPuestoId || e.Idpuesto.Value == jefeTiendaPuestoId)
                        .Where(e => e.Estaciones.Any(est => est.Idestacion == empleado.Idestacion))
                        .ToListAsync();

                    encargados = supervisoresRelacionados;
                }
                else if (empleado.Division == "ESTACIONES")
                {
                    var estacionesDelEmpleado = empleado.Estaciones.Select(est => est.Idestacion).ToList();

                    // Buscar gerentes que estén asignados al menos a una de las mismas estaciones
                    var jefesRelacionados = await context.Empleados
                        .Include(e => e.Estaciones) // Incluir estaciones para filtrar
                        .Include(e => e.Persona)
                        .Where(e => e.Idpuesto.HasValue && jefeDeTurnoPuestoId == e.Idpuesto.Value)
                        .Where(e => e.Estaciones.Any(est => est.Idestacion == empleado.Idestacion))
                        .ToListAsync();

                    encargados = jefesRelacionados;
                }
                else if (empleado.Division == "ADMINISTRATIVO")
                {
                    var gerentesRelacionados = await context.Empleados
                        .Include(e => e.Estaciones) // Incluir estaciones si las usarás más adelante
                        .Include(e => e.Persona)    // Incluir persona si la necesitas más adelante
                        .Where(e => e.Idpuesto.HasValue && gerentePuestoIds.Contains(e.Idpuesto.Value)) // Filtrar por puestos
                        .Where(e => e.Division == "ADMINISTRATIVO")
                        .Where(e => e.Iddepartamento == empleado.Iddepartamento) // Mismo departamento
                        .ToListAsync();

                    encargados = gerentesRelacionados;
                }

                // Ejecutamos la consulta y obtenemos la lista

                return Ok(encargados);

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al guardar respuestas: {ex.Message}");
            }
        }

        [HttpGet("UserHasAnsweredAllGroups")]
        public async Task<ActionResult<bool>> UserHasAnsweredAllGroups()
        {
            // 1. Obtener el usuario actual
            var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            if (user == null)
            {
                // Podrías regresar false si no está autenticado
                return false;
            }

            // 2. Obtener cuántos grupos hay en total
            var totalGrupos = await context.QGrupo.CountAsync();

            // 3. Obtener los ID de grupo que el usuario ya respondió
            var gruposRespondidos = await (
                from respuesta in context.QRespuesta
                join pregunta in context.QPreguntas
                    on respuesta.PreguntaId equals pregunta.Idpregunta
                where respuesta.UsuarioId == user.Id
                select pregunta.GrupoId
            ).Distinct().ToListAsync();

            // 4. Evaluar si respondió al menos una pregunta en cada grupo
            bool userHasAnsweredAllGroups = (gruposRespondidos.Count >= totalGrupos);

            // 5. Regresar el valor
            return Ok(userHasAnsweredAllGroups);
        }
    }
}
