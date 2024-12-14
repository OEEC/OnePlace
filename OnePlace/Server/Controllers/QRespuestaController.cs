using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnePlace.Server.Data;
using OnePlace.Shared.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using OnePlace.Shared.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace OnePlace.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QRespuestaController : ControllerBase
    {
        private readonly oneplaceContext context;
        private readonly UserManager<ApplicationUser> userManager;

        public QRespuestaController(oneplaceContext context, UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        [HttpPost("guardar")]
        public async Task<IActionResult> GuardarRespuestas([FromBody] List<QRespuestaDTO> respuestasDto)
        {
            //if (respuestasDto == null || !respuestasDto.Any())
            //{
            //    return BadRequest("No hay respuestas para guardar.");
            //}

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
                        //else if (respuestaExistente != null && respuestaExistente.Pregunta.TipoPreguntaId == 4) 
                        //{
                        //    foreach (var item in respuestasDto)
                        //    {
                                
                        //    }
                        //}
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
    }
}
