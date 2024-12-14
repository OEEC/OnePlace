using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using OnePlace.Server.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using OnePlace.Shared.DTOs;
using System.Collections.Generic;
using OnePlace.Shared.Entidades;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using OnePlace.Client.Pages.QuizAmbiente;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http;

namespace OnePlace.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PreguntaController : ControllerBase
    {
        private readonly oneplaceContext context;
        private readonly UserManager<ApplicationUser> userManager;

        public PreguntaController(oneplaceContext context, UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        [HttpGet("preguntas/{groupId}")]
        public async Task<ActionResult> GetPreguntasPorGrupo(int groupId)
        {
            try
            {
                // Obtener el usuario actual
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                if (user == null)
                    return BadRequest(new { mensaje = "Usuario no autenticado." });
                var preguntasConRespuestas = await context.QPreguntas
                    .Include(x => x.ListaRepuesta)
                    .Include( p => p.ListTipoRspuesta)
                    .Where(p => p.GrupoId == groupId)
                    .Select(p => new QPreguntaConRespuestasDTO
                    {
                        Idpregunta = p.Idpregunta,
                        GrupoId = p.GrupoId,
                        Pregunta = p.Pregunta,
                        TipoPreguntaId = p.TipoPreguntaId,
                        TipoRespuestas = p.ListTipoRspuesta.Select( s => new QTipoRespuestaDTO
                        {
                            IdTipoRespuesta = s.IdTipoRespuesta,
                            TipoPreguntaId = s.TipoPreguntaId,
                            Cantidad = s.Cantidad,
                            Texto = s.Texto
                        }).ToList(),
                        RespuestaUsuario = p.ListaRepuesta
                            .Where(r => r.UsuarioId == user.Id) // Filtrar respuestas por usuario actual
                            .Select(r => new QRespuestaDTO
                            {
                                PreguntaId = r.PreguntaId,
                                IdRespuesta = r.IdRespuesta,
                                Respuesta = r.Respuesta,
                                UsuarioId = r.UsuarioId
                            })
                            .ToList()
                    })
                    .ToListAsync(); // Obtener la lista completa

                return Ok(preguntasConRespuestas);
            }
            catch (Exception e)
            {
                return BadRequest(new { mensaje = e.Message });
            }
        }

        [HttpGet("tiporespuesta/{preguntaId}")]
        public async Task<ActionResult> GetTipoPregunta(int preguntaId)
        {
            try
            {
                // Obtener el usuario actual
                var tipoRespuesta = await context.QTipoRespuesta
                    .Include(p => p.TipoPregunta)
                    .Where(p => p.PreguntaId == preguntaId)
                    .ToListAsync();

                return Ok(tipoRespuesta);
            }
            catch (Exception e)
            {
                return BadRequest(new { mensaje = e.Message });
            }
        }

    }
}
