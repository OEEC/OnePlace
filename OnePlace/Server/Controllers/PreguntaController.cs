using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using OnePlace.Client.Pages.QuizAmbiente;
using OnePlace.Server.Data;
using OnePlace.Shared.DTOs;
using OnePlace.Shared.DTOs.Modelos;
using OnePlace.Shared.DTOs.Reportes;
using OnePlace.Shared.Entidades;
using OnePlace.Shared.Enums;
using OnePlace.Shared.Extensiones;
using OnePlace.Shared.IdentityModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace OnePlace.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PreguntaController : ControllerBase
    {
        private readonly oneplaceContext context;
        private readonly UserManager<IdentityUsuario> userManager;
        private readonly IMapper mapper;

        public PreguntaController(oneplaceContext context, UserManager<IdentityUsuario> userManager, IMapper mapper)
        {
            this.context = context;
            this.userManager = userManager;
            this.mapper = mapper;
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
                    .Include(p => p.ListTipoRspuesta)
                    .Where(p => p.GrupoId == groupId)
                    .Select(p => new QPreguntaConRespuestasDTO
                    {
                        Idpregunta = p.Idpregunta,
                        GrupoId = p.GrupoId,
                        Pregunta = p.Pregunta,
                        TipoPreguntaId = p.TipoPreguntaId,
                        TipoRespuestas = p.ListTipoRspuesta.Select(s => new QTipoRespuestaDTO
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

        [HttpGet("respuestas")]
        public async Task<ActionResult> GetRespuestas([FromQuery] QPreguntasFiltroDTO filtroDTO)
        {
            //consulta de respuestas entre fechas
            var respuestas = context.QRespuesta
                .AsNoTracking()
                .IgnoreAutoIncludes()
                .Where(x => x.fecha >= filtroDTO.Fecha_Inicio && x.fecha <= filtroDTO.Fecha_Fin)
                .Include(x => x.Pregunta.Grupo)
                .Include(x => x.Usuario.Empleado.Estacion.ZonaR)
                .Include(x => x.Usuario.Empleado.Estacion.Empleados)
                .ThenInclude(x => x.Persona)
                .Include(x => x.Usuario.Empleado.Estacion.Empleados)
                .ThenInclude(x => x.Puesto)
                .AsQueryable();

            //filtros de respuestas
            if (filtroDTO.ZonaId != 0)
                respuestas = respuestas.Where(x => x.Usuario.Empleado.ZonaId == filtroDTO.ZonaId);

            if (filtroDTO.EstacionId != 0)
                respuestas = respuestas.Where(x => x.Usuario.Empleado.Idestacion == filtroDTO.EstacionId);

            if (filtroDTO.DepartamentoId != 0)
                respuestas = respuestas.Where(x => x.Usuario.Empleado.Iddepartamento == filtroDTO.DepartamentoId);

            if (filtroDTO.TipoQuiz.Equals(TipoQuiz.Quiz))
                respuestas = respuestas.Where(x => x.Pregunta.GrupoId != 8);

            if (filtroDTO.TipoQuiz.Equals(TipoQuiz.QuizTrato))
                respuestas = respuestas.Where(x => x.Pregunta.GrupoId == 6);

            if (filtroDTO.TipoQuiz.Equals(TipoQuiz.Recomendacion))
                respuestas = respuestas.Where(x => x.Pregunta.GrupoId == 8 || x.PreguntaId == 30);

            //ejecucion de la consulta
            var respuestaslist = await respuestas.ToListAsync();

            //creacion de responce de acuerdo al tipo de quiz seleccionado
            if (filtroDTO.TipoQuiz.Equals(TipoQuiz.Quiz))
            {
                var respuestagroup = respuestaslist.GroupBy(x => (x.Usuario.Empleado.Estacion.Nombre, x.Usuario.Empleado.Estacion.ZonaR.Zona1),
                    x => x, (baseres, res) => new QuizRespuestasDTO
                    {
                        EstacionTienda = $"{baseres.Nombre} - {baseres.Zona1}",
                        Organizacion = res.Where(x => x.Pregunta.GrupoId == 1 && x.Pregunta.TipoPreguntaId == 1).Sum(x => x.Respuesta.ToInt()),
                        Comunacion = res.Where(x => x.Pregunta.GrupoId == 2 && x.Pregunta.TipoPreguntaId == 1).Sum(x => x.Respuesta.ToInt()),
                        Realizacion = res.Where(x => x.Pregunta.GrupoId == 3 && x.Pregunta.TipoPreguntaId == 1).Sum(x => x.Respuesta.ToInt()),
                        Motivacion = res.Where(x => x.Pregunta.GrupoId == 4 && x.Pregunta.TipoPreguntaId == 1).Sum(x => x.Respuesta.ToInt()),
                        Ambiente = res.Where(x => x.Pregunta.GrupoId == 5 && x.Pregunta.TipoPreguntaId == 1).Sum(x => x.Respuesta.ToInt()),
                        Trato = res.Where(x => x.Pregunta.GrupoId == 6 && x.Pregunta.TipoPreguntaId == 1).Sum(x => x.Respuesta.ToInt()),
                        Seguridad = res.Where(x => x.Pregunta.GrupoId == 7 && x.Pregunta.TipoPreguntaId == 1).Sum(x => x.Respuesta.ToInt()),
                        NoPersonas = res.GroupBy(z => z.UsuarioId).Select(x => x.Key).Count(),
                    }).ToList();

                if (filtroDTO.Excel)
                {
                    ExcelPackage.LicenseContext = LicenseContext.Commercial;
                    ExcelPackage excel = new();
                    var ws = excel.Workbook.Worksheets.Add("Quiz");

                    ws.Cells["A1"].LoadFromCollection(respuestagroup, true, TableStyles.Medium2);
                    ws.Cells[1, 1, ws.Dimension.End.Row, ws.Dimension.End.Column].AutoFitColumns();

                    return Ok(excel.GetAsByteArray());
                }

                return Ok(respuestagroup);
            }
            else if (filtroDTO.TipoQuiz.Equals(TipoQuiz.QuizTrato))
            {
                var preguntasencabecados = await context.QPreguntas
                    .AsNoTracking()
                    .Include(x => x.Grupo)
                    .Where(x => x.GrupoId == 6 && x.Estatus == 1 && x.Pregunta != "Nombre de jefe directo:")
                    .OrderBy(x => x.Idpregunta)
                    .Select(x => x.Pregunta)
                    .ToListAsync();

                var respuestasgrouptrato = respuestaslist.GroupBy(x => (x.UsuarioId, x.Usuario.Empleado.Estacion.Nombre, x.Usuario.Empleado.Estacion.ZonaR.Zona1),
                    x => x, (baseres, res) => new QuizRespuestasTratoDTO
                    {
                        EstacionTienda = $"{baseres.Nombre} - {baseres.Zona1}",
                        Respuestas = res.Where(x => x.Pregunta.TipoPreguntaId == 1 && x.UsuarioId == baseres.UsuarioId)
                                        .OrderBy(x => x.PreguntaId)
                                        .Select(x => x.Respuesta.ToInt())
                                        .ToList(),
                        Suma = res.Where(x => x.Pregunta.TipoPreguntaId == 1 && x.UsuarioId == baseres.UsuarioId)
                                  .OrderBy(x => x.PreguntaId)
                                  .Select(x => x.Respuesta.ToInt())
                                  .Sum(x => x),
                        Encargado = res.FirstOrDefault(x => x.PreguntaId == 30 && x.UsuarioId == baseres.UsuarioId)?.Usuario.Empleado.Estacion.Empleados.FirstOrDefault(
                            x => x.Idempleado == res.FirstOrDefault(x => x.PreguntaId == 30 && x.UsuarioId == baseres.UsuarioId)?.Respuesta.ToInt()
                            )?.Persona.FullName ?? string.Empty,
                    }).ToList();

                QuizPreguntasTratoDTO Preguntas = new()
                {
                    Preguntas = preguntasencabecados,
                    Respuestas = respuestasgrouptrato
                };

                if (filtroDTO.Excel)
                {
                    DataTable table = new("QuizTrato");
                    DataColumn column = new()
                    {
                        DataType = System.Type.GetType("System.String"),
                        ColumnName = nameof(QuizRespuestasTratoDTO.EstacionTienda),
                        Caption = "Estacion / Tienda"
                    };
                    table.Columns.Add(column);
                    for (int i = 0; i < Preguntas.Preguntas.Count; i++)
                    {
                        column = new()
                        {
                            DataType = System.Type.GetType("System.Int32"),
                            ColumnName = $"Pregunta{i}",
                            Caption = Preguntas.Preguntas[i]
                        };
                        table.Columns.Add(column);
                    }
                    column = new()
                    {
                        DataType = System.Type.GetType("System.Int32"),
                        ColumnName = nameof(QuizRespuestasTratoDTO.Suma),
                        Caption = "Suma"
                    };
                    table.Columns.Add(column);
                    column = new()
                    {
                        DataType = System.Type.GetType("System.String"),
                        ColumnName = nameof(QuizRespuestasTratoDTO.Encargado),
                        Caption = "Encargado"
                    };
                    table.Columns.Add(column);
                    for (int i = 0; i < Preguntas.Respuestas.Count; i++)
                    {
                        DataRow row = table.NewRow();
                        row[nameof(QuizRespuestasTratoDTO.EstacionTienda)] = Preguntas.Respuestas[i].EstacionTienda;
                        row[nameof(QuizRespuestasTratoDTO.Suma)] = Preguntas.Respuestas[i].Suma;
                        for (int j = 0; j < Preguntas.Respuestas[i].Respuestas.Count; j++)
                        {
                            row[$"Pregunta{j}"] = Preguntas.Respuestas[i].Respuestas[j];
                        }
                        row[nameof(QuizRespuestasTratoDTO.Encargado)] = Preguntas.Respuestas[i].Encargado;
                        table.Rows.Add(row);
                    }

                    ExcelPackage.LicenseContext = LicenseContext.Commercial;
                    ExcelPackage excel = new();

                    var ws = excel.Workbook.Worksheets.Add("Quiz Trato");

                    ws.Cells["A1"].LoadFromDataTable(table, true, TableStyles.Medium2);
                    ws.Cells[1, 1, ws.Dimension.End.Row, ws.Dimension.End.Column].AutoFitColumns();

                    return Ok(excel.GetAsByteArray());
                }

                return Ok(Preguntas);
            }
            else if (filtroDTO.TipoQuiz.Equals(TipoQuiz.Recomendacion))
            {
                //conteo de pregutas totales en base a las respuestas de los usuarios
                var countpreguntas = respuestaslist.Where(x => x.Pregunta.Estatus == 1 && x.PreguntaId != 30).GroupBy(x => x.PreguntaId).Count();

                //agrupacion de las preguntas en base a las respuestas de los usuarios
                var respuestasdtos = respuestaslist.GroupBy(x => new
                {
                    x.Pregunta.Idpregunta,
                    x.Pregunta.Pregunta,
                    x.UsuarioId,
                    x.Pregunta.TipoPreguntaId,
                    x.Pregunta.Estatus
                }, x => x, (baseres, res) => new QzPreguntaDTO
                {
                    Idpregunta = baseres.Idpregunta,
                    UsuarioId = baseres.UsuarioId,
                    Pregunta = baseres.Pregunta,
                    TipoPreguntaId = baseres.TipoPreguntaId,
                    Estatus = baseres.Estatus,
                    //agregamos las respuestas del usuario a la pregunta correspondiente
                    ListaRepuesta = res.Where(x => x.PreguntaId == baseres.Idpregunta && x.UsuarioId == baseres.UsuarioId)
                                       .Select(x => mapper.Map<QzRespuestaSimpleDTO>(x))
                                       .ToList()
                })
                .OrderBy(x => x.Idpregunta);

                //igualamos los items de las respuestas a 2 en caso de de que sea una pregunta multiple
                //estos nos evita problemas al momento de mostrar las respuestas, ya que saldrian desfasadas de sus columnas correspondientes
                var resdtos = respuestasdtos.Select(x =>
                {
                    if (x.TipoPreguntaId == 4)
                    {
                        if (x.ListaRepuesta.Count < 2)
                        {
                            for (int i = x.ListaRepuesta.Count; i < 2; i++)
                            {
                                //se agrega una respuesta vacia para la igualacion
                                x.ListaRepuesta.Add(new()
                                {
                                    PreguntaId = x.Idpregunta,
                                    Respuesta = string.Empty,
                                    UsuarioId = x.UsuarioId,
                                });
                            }
                        }
                    }
                    return x;
                });

                //agrupamos las respuestas por estacion y usuario, y agregamos las preguntas que le corresponden al usuario
                var quizrecomendacion = respuestaslist.GroupBy(x => (x.Usuario.Empleado.Estacion.Nombre, x.Usuario.Empleado.Estacion.ZonaR.Zona1, x.UsuarioId),
                    x => x, (baseres, res) => new QuizRespuestasRecomendacionDTO
                    {
                        EstacionTienda = $"{baseres.Nombre} - {baseres.Zona1}",
                        Preguntas = resdtos.Where(x => x.UsuarioId == baseres.UsuarioId && x.Estatus == 1 && x.Idpregunta != 30).ToList(),
                        Fecha = res.FirstOrDefault(x => x.UsuarioId == baseres.UsuarioId)?.fecha ?? DateTime.MinValue,
                        Calificado = res.FirstOrDefault(x => x.UsuarioId == baseres.UsuarioId && x.PreguntaId == 30)?.Usuario.Empleado.Estacion.Empleados.FirstOrDefault(
                            x => x.Idempleado ==
                            (res.FirstOrDefault(x => x.UsuarioId == baseres.UsuarioId && x.PreguntaId == 30)?.Respuesta.ToInt() ?? 0))?.Persona.FullName ?? string.Empty,
                        Puesto = res.FirstOrDefault(x => x.UsuarioId == baseres.UsuarioId && x.PreguntaId == 30)?.Usuario.Empleado.Estacion.Empleados.FirstOrDefault(
                            x => x.Idempleado ==
                            (res.FirstOrDefault(x => x.UsuarioId == baseres.UsuarioId && x.PreguntaId == 30)?.Respuesta.ToInt() ?? 0))?.Puesto?.Puesto1 ?? string.Empty
                    }).ToList();

                QuizPreguntasRecomendacionDTO preguntas = new()
                {
                    CantidadPreguntas = countpreguntas,
                    Respuestas = quizrecomendacion
                };

                if (filtroDTO.Excel)
                {
                    DataTable table = new("QuizRecomendacion");
                    DataColumn column = new()
                    {
                        DataType = System.Type.GetType("System.String"),
                        ColumnName = nameof(QuizRespuestasTratoDTO.EstacionTienda),
                        Caption = "Estacion / Tienda"
                    };
                    table.Columns.Add(column);
                    for (int i = 0; i < preguntas.CantidadPreguntas; i++)
                    {
                        column = new()
                        {
                            DataType = System.Type.GetType("System.String"),
                            ColumnName = $"Pregunta{i}",
                            Caption = $"Preunta{i + 1}"
                        };
                        table.Columns.Add(column);
                        column = new()
                        {
                            DataType = System.Type.GetType("System.String"),
                            ColumnName = $"Respuesta{i}",
                            Caption = $"Respuesta {i + 1}"
                        };
                        table.Columns.Add(column);
                        column = new()
                        {
                            DataType = System.Type.GetType("System.String"),
                            ColumnName = $"Motivo{i}",
                            Caption = $"Motivo {i + 1}"
                        };
                        table.Columns.Add(column);
                    }
                    column = new()
                    {
                        DataType = System.Type.GetType("System.String"),
                        ColumnName = nameof(QuizRespuestasRecomendacionDTO.Fecha),
                        Caption = "Fecha"
                    };
                    table.Columns.Add(column);
                    column = new()
                    {
                        DataType = System.Type.GetType("System.String"),
                        ColumnName = nameof(QuizRespuestasRecomendacionDTO.Calificado),
                        Caption = "Calificado"
                    };
                    table.Columns.Add(column);
                    column = new()
                    {
                        DataType = System.Type.GetType("System.String"),
                        ColumnName = nameof(QuizRespuestasRecomendacionDTO.Puesto),
                        Caption = "Puesto"
                    };
                    table.Columns.Add(column);
                    for (int i = 0; i < preguntas.Respuestas.Count; i++)
                    {
                        DataRow row = table.NewRow();
                        row[nameof(QuizRespuestasRecomendacionDTO.EstacionTienda)] = preguntas.Respuestas[i].EstacionTienda;
                        for (int j = 0; j < preguntas.Respuestas[i].Preguntas.Count; j++)
                        {
                            row[$"Pregunta{j}"] = preguntas.Respuestas[i].Preguntas[j].Pregunta;
                            for (int k = 0; k < preguntas.Respuestas[i].Preguntas[j].ListaRepuesta.Count; k++)
                            {
                                if (k == 0)
                                    row[$"Respuesta{j}"] = preguntas.Respuestas[i].Preguntas[j].ListaRepuesta[k].Respuesta;
                                else
                                    row[$"Motivo{j}"] = preguntas.Respuestas[i].Preguntas[j].ListaRepuesta[k].Respuesta;
                            }
                        }
                        row[nameof(QuizRespuestasRecomendacionDTO.Fecha)] = preguntas.Respuestas[i].Fecha;
                        row[nameof(QuizRespuestasRecomendacionDTO.Calificado)] = preguntas.Respuestas[i].Calificado;
                        row[nameof(QuizRespuestasRecomendacionDTO.Puesto)] = preguntas.Respuestas[i].Puesto;
                        table.Rows.Add(row);
                    }

                    ExcelPackage.LicenseContext = LicenseContext.Commercial;
                    ExcelPackage excel = new();

                    var ws = excel.Workbook.Worksheets.Add("Quiz Trato");

                    ws.Cells["A1"].LoadFromDataTable(table, true, TableStyles.Medium2);
                    ws.Cells[1, 1, ws.Dimension.End.Row, ws.Dimension.End.Column].AutoFitColumns();

                    return Ok(excel.GetAsByteArray());
                }


                return Ok(preguntas);
            }

            return NoContent();
        }
    }
}
