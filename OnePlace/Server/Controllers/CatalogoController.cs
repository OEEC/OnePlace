using iTextSharp.text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using OnePlace.Server.Data;
using OnePlace.Shared.Entidades.SimsaCore;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace OnePlace.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrador")]
    public class CatalogoController : ControllerBase
    {
        private readonly oneplaceContext context;

        public CatalogoController(oneplaceContext context)
        {
            this.context = context;
        }

        [HttpGet("departamentos")]
        public ActionResult Departamentos([FromQuery] Departamento departamento)
        {
            try
            {
                var departamentos = context.Departamentos.Where(x => x.Idestatus == 1).OrderBy(x => x.Departamento1).AsQueryable();

                if (!string.IsNullOrEmpty(departamento.Departamento1))
                    departamentos = departamentos.Where(x => x.Departamento1.ToLower().Contains(departamento.Departamento1.ToLower()));

                return Ok(departamentos);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("areas")]
        public ActionResult Areas([FromQuery] Area area)
        {
            try
            {
                var areas = context.Areas.Where(x => x.Idestatus == 1).OrderBy(x => x.Area1).AsQueryable();

                if (!string.IsNullOrEmpty(area.Area1))
                    areas = areas.Where(x => x.Area1.ToLower().Contains(area.Area1.ToLower()));

                return Ok(areas);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("puestos")]
        public ActionResult Puestos([FromQuery] Puesto puesto)
        {
            try
            {
                var puestos = context.Puestos.OrderBy(x => x.Puesto1).AsQueryable();

                if (!string.IsNullOrEmpty(puesto.Puesto1))
                    puestos = puestos.Where(x => x.Puesto1.ToLower().Contains(puesto.Puesto1.ToLower()));

                return Ok(puestos);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("tiendas")]
        public ActionResult Tiendas([FromQuery] Tienda tienda)
        {
            try
            {
                var tiendas = context.Tienda.OrderBy(x => x.Nombre).AsQueryable();

                if (!string.IsNullOrEmpty(tienda.Nombre))
                    tiendas = tiendas.Where(x => x.Nombre.ToLower().Contains(tienda.Nombre.ToLower()));

                return Ok(tiendas);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("estaciones")]
        public ActionResult Estaciones([FromQuery] Estacion estacion)
        {
            try
            {
                var estaciones = context.Estaciones.Where(x => x.Estatus == 1).Select(x => new { Nombre = x.Nombre, Idestacion = x.Idestacion }).OrderBy(x => x.Nombre).AsQueryable();

                if (!string.IsNullOrEmpty(estacion.Nombre))
                    estaciones = estaciones.Where(x => x.Nombre.ToLower().Contains(estacion.Nombre.ToLower()));

                return Ok(estaciones);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("zonas")]
        public ActionResult Zonas([FromQuery] Zona zona)
        {
            try
            {
                var zonas = context.Zonas.Where(x => x.Idestatus == 1).OrderBy(x => x.Zona1).AsQueryable();

                if (!string.IsNullOrEmpty(zona.Zona1))
                    zonas = zonas.Where(x => x.Zona1.ToLower().Contains(zona.Zona1.ToLower()));

                return Ok(zonas);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("empresas")]
        public ActionResult Empresas([FromQuery] Empresa empresa)
        {
            try
            {
                var empresas = context.Empresas.Where(x => x.Idestatus == 1).OrderBy(x => x.Razonsocial).AsQueryable();

                if (!string.IsNullOrEmpty(empresa.Razonsocial))
                    empresas = empresas.Where(x => x.Razonsocial.ToLower().Contains(empresa.Razonsocial.ToLower()));

                return Ok(empresas);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("excel")]
        public ActionResult Excel()
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.Commercial;
                var excel = new ExcelPackage();
                var ws_divisiones = excel.Workbook.Worksheets.Add("Divisiones");
                var ws_zonas = excel.Workbook.Worksheets.Add("Zonas");
                var ws_empresas = excel.Workbook.Worksheets.Add("Empresas");
                var ws_estaciones = excel.Workbook.Worksheets.Add("Estaciones");
                var ws_departamentos = excel.Workbook.Worksheets.Add("Departamentos");
                var ws_areas = excel.Workbook.Worksheets.Add("Areas");
                var ws_puestos = excel.Workbook.Worksheets.Add("Puestos");

                var zonas = context.Zonas.IgnoreAutoIncludes().Where(x => x.Idestatus == 1).Select(x => new { Zona = x.Zona1 }).OrderBy(x => x.Zona).ToList();
                ws_zonas.Cells["A1"].LoadFromCollection(zonas, x => { x.PrintHeaders = true; x.TableStyle = TableStyles.Medium2; });
                ws_zonas.Cells[1, 1, ws_zonas.Dimension.End.Row, ws_zonas.Dimension.End.Column].AutoFitColumns();

                var empresas = context.Empresas.IgnoreAutoIncludes().Where(x => x.Idestatus == 1).Select(x => new { Empresa = x.Razonsocial }).OrderBy(x => x.Empresa).ToList();
                ws_empresas.Cells["A1"].LoadFromCollection(empresas, x => { x.PrintHeaders = true; x.TableStyle = TableStyles.Medium2; });
                ws_empresas.Cells[1, 1, ws_empresas.Dimension.End.Row, ws_empresas.Dimension.End.Column].AutoFitColumns();

                var estaciones = context.Estaciones.IgnoreAutoIncludes().Where(x => x.Estatus == 1).Select(x => new { Estacion = x.Nombre }).OrderBy(x => x.Estacion).ToList();
                ws_estaciones.Cells["A1"].LoadFromCollection(estaciones, x => { x.PrintHeaders = true; x.TableStyle = TableStyles.Medium2; });
                ws_estaciones.Cells[1, 1, ws_estaciones.Dimension.End.Row, ws_estaciones.Dimension.End.Column].AutoFitColumns();

                var departamentos = context.Departamentos.IgnoreAutoIncludes().Where(x => x.Idestatus == 1).Select(x => new { Departemento = x.Departamento1 }).OrderBy(x => x.Departemento).ToList();
                ws_departamentos.Cells["A1"].LoadFromCollection(departamentos, x => { x.PrintHeaders = true; x.TableStyle = TableStyles.Medium2; });
                ws_departamentos.Cells[1, 1, ws_departamentos.Dimension.End.Row, ws_departamentos.Dimension.End.Column].AutoFitColumns();

                var areas = context.Areas.IgnoreAutoIncludes().Where(x => x.Idestatus == 1).Select(x => new { Area = x.Area1 }).OrderBy(x => x.Area).ToList();
                ws_areas.Cells["A1"].LoadFromCollection(areas, x => { x.PrintHeaders = true; x.TableStyle = TableStyles.Medium2; });
                ws_areas.Cells[1, 1, ws_areas.Dimension.End.Row, ws_areas.Dimension.End.Column].AutoFitColumns();

                var puestos = context.Puestos.IgnoreAutoIncludes().Select(x => new { Puesto = x.Puesto1 }).OrderBy(x => x.Puesto).ToList();
                ws_puestos.Cells["A1"].LoadFromCollection(puestos, x => { x.PrintHeaders = true; x.TableStyle = TableStyles.Medium2; });
                ws_puestos.Cells[1, 1, ws_puestos.Dimension.End.Row, ws_puestos.Dimension.End.Column].AutoFitColumns();

                List<ExpandoObject> divisiones = new();
                dynamic division = new ExpandoObject();
                division.Division = "ESTACIONES";
                divisiones.Add(division);

                division = new ExpandoObject();
                division.Division = "TIENDAS";
                divisiones.Add(division);
                
                ws_divisiones.Cells["A1"].LoadFromCollection(divisiones, x => { x.PrintHeaders = true; x.TableStyle = TableStyles.Medium2; });
                ws_divisiones.Cells[1, 1, ws_divisiones.Dimension.End.Row, ws_divisiones.Dimension.End.Column].AutoFitColumns();

                return Ok(excel.GetAsByteArray());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
