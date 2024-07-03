using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OnePlace.Server.Data;
using OnePlace.Server.Helpers;
using OnePlace.Shared.DTOs;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OnePlace.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ActividadController : ControllerBase
    {
        private readonly oneplaceContext context;

        public ActividadController(oneplaceContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<ActionResult> GetAcciones([FromQuery] ContadorActividadDTO contador)
        {
            try
            {
                var acciones = context.Contador
                    .Where(x => x.Fecha.Date >= contador.Fecha_Inicio && x.Fecha.Date <= contador.Fecha_Fin)
                    .Include(x => x.Estacion)
                    .Include(x => x.Empleado)
                    .ThenInclude(x => x.Persona)
                    .Include(x => x.Accion)
                    .OrderByDescending(x => x.Fecha)
                    //.Select(x=> new ContadorActividadDTO()
                    //{
                    //    Estacion = x.Estacion.Nombre,
                    //    Empleado = x.Empleado.Noemp,
                    //    Persona = x.Empleado.Persona.FullName(),
                    //    Accion = x.Accion.Descripcion,
                    //    Fecha_Registro = x.Fecha
                    //})
                    .GroupBy(x => new { x.Estacion.Nombre, x.Fecha.Date })
                    .Select(x => new ContadorActividadEstacionDTO()
                    {
                        Estacion = x.Key.Nombre,
                        Cantidad = x.Count(),
                        Fecha = x.Key.Date
                    })
                    .AsQueryable();

                if (!string.IsNullOrEmpty(contador.Estacion) || !string.IsNullOrWhiteSpace(contador.Estacion))
                    acciones = acciones.Where(x => x.Estacion.ToLower().Contains(contador.Estacion.ToLower()));

                //if (!string.IsNullOrEmpty(contador.Empleado) || !string.IsNullOrWhiteSpace(contador.Empleado))
                //    acciones = acciones.Where(x => x.Empleado.ToLower().Contains(contador.Empleado.ToLower()));

                //if (!string.IsNullOrEmpty(contador.Persona) || !string.IsNullOrWhiteSpace(contador.Persona))
                //    acciones = acciones.Where(x => x.Persona.ToLower().Contains(contador.Persona.ToLower()));

                if (contador.Excel)
                {
                    ExcelPackage.LicenseContext = LicenseContext.Commercial;
                    ExcelPackage excel = new();

                    excel.Workbook.Worksheets.Add("Registro");

                    ExcelWorksheet worksheet = excel.Workbook.Worksheets.First();

                    worksheet.Cells["A1"].LoadFromCollection(acciones, op =>
                    {
                        op.PrintHeaders = true;
                        op.TableStyle = OfficeOpenXml.Table.TableStyles.Medium2;
                    });

                    worksheet.Cells[1, 3, worksheet.Dimension.End.Row, 3].Style.Numberformat.Format = "dd/MM/yyyy";

                    worksheet.Cells[1, 1, worksheet.Dimension.End.Row, worksheet.Dimension.End.Column].AutoFitColumns();

                    return Ok(excel.GetAsByteArray());
                }

                await HttpContext.InsertarParametrosPaginacion(acciones, contador.Registros_por_pagina, contador.Pagina);

                if (HttpContext.Response.Headers.TryGetValue("pagina", out Microsoft.Extensions.Primitives.StringValues value))
                    if (!string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value) && value != contador.Pagina)
                        contador.Pagina = int.Parse(value);

                acciones = acciones.Skip((contador.Pagina - 1) * contador.Registros_por_pagina).Take(contador.Registros_por_pagina);

                return Ok(acciones);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
