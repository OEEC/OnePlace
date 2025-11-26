using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Engineering;
using OfficeOpenXml.Table;
using OnePlace.Server.Data;
using OnePlace.Server.Helpers;
using OnePlace.Shared.DTOs;
using OnePlace.Shared.DTOs.Modelos;
using OnePlace.Shared.Entidades.SimsaCore;
using OnePlace.Shared.IdentityModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OnePlace.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrador")]
    public class EmpleadoController : ControllerBase
    {
        private readonly oneplaceContext context;
        private readonly UserManager<IdentityUsuario> _userManager;
        private readonly ILogger<EmpleadoController> logger;
        public EmpleadoController(oneplaceContext context, UserManager<IdentityUsuario> userManager, ILogger<EmpleadoController> logger)
        {
            this.context = context;
            _userManager = userManager;
            this.logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult> Crear_Empleado([FromBody] EmpleadoPostDTO dto)
        {
            try
            {
                if (dto is null) { return BadRequest(); }

                var errors = string.Empty;

                Empleado empleado = new()
                {
                    Iddepartamento = dto.IdDepartamento,
                    Idestacion = dto.IdEstacion,
                    Idpuesto = dto.IdPuesto,
                    ZonaId = dto.IdZona,
                    Noemp = dto.NoEmpleado,
                    Division = dto.Division,
                    Idestatus = "1",
                    Fchalta = dto.Fchalta,
                    Persona = new()
                    {
                        Nombre = dto.Nombre,
                        Ape_pat = dto.ApellidoPat,
                        Ape_mat = dto.ApellidoMat
                    }
                };

                if (context.Empleados.Any(x => x.Noemp.ToLower().Equals(empleado.Noemp.ToLower()) && x.ZonaId == empleado.ZonaId))
                    return BadRequest("Ya existe un emplado con el mismo No de empleado");

                if (!Validar_Contraseña(dto.Password, out errors))
                    return BadRequest(errors);

                context.Add(empleado);
                await context.SaveChangesAsync();

                if (!context.Users.Any(x => x.UserName == dto.Usuario))
                {
                    //Asigna valores a objeto usuario
                    var user = new IdentityUsuario
                    {
                        UserName = dto.Usuario,
                        noemp = empleado.Noemp,
                        Idempleado = empleado.Idempleado,
                        Nombre = empleado.Persona.Nombre,
                        ApellidoMaterno = empleado.Persona.Ape_mat,
                        ApellidoPaterno = empleado.Persona.Ape_pat,
                        ContraseñaTextoPlano = dto.Password,
                        Activo = true
                    };

                    var result = await _userManager.CreateAsync(user, user.ContraseñaTextoPlano);

                    if (result.Succeeded)
                        await _userManager.AddToRoleAsync(user, "Usuario");
                    else
                        return BadRequest(result.Errors);
                }

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("file")]
        public async Task<ActionResult<IList<UploadResult>>> Subir_Excel_Empleados([FromForm] IEnumerable<IFormFile> files)
        {
            var maxAllowedFiles = 3;
            long maxFileSize = 1024 * 15 * 1024;
            var filesProcessed = 0;
            List<UploadResult> uploadResults = new();
            bool hasErrors = false;
            string Errors = string.Empty;

            #region Pre-carga de datos
            var estaciones = await context.Estaciones.AsNoTracking().Where(x => x.Estatus == 1).ToListAsync();
            var zonas = await context.Zonas.AsNoTracking().Where(x => x.Idestatus == 1).ToListAsync();
            var departamentos = await context.Departamentos.AsNoTracking().Where(x => x.Idestatus == 1).ToListAsync();
            var puestos = await context.Puestos.AsNoTracking().ToListAsync();
            var empresas = await context.Empresas.AsNoTracking().Where(x => x.Idestatus == 1).ToListAsync();
            var usernames = await context.Users.AsNoTracking().Select(x => new { x.UserName, x.ContraseñaTextoPlano }).ToListAsync();
            var empleados = await context.Empleados.AsNoTracking().Select(x => new { x.Idempleado, x.Noemp, x.Idpersona }).ToListAsync();
            #endregion

            foreach (var file in files)
            {
                var uploadResult = new UploadResult();

                var unthrustedFileName = file.FileName;
                uploadResult.FileName = unthrustedFileName;
                var thrustFileName = WebUtility.HtmlEncode(unthrustedFileName);

                if (filesProcessed < maxAllowedFiles)
                {
                    try
                    {
                        if (file.Length == 0)
                        {
                            logger.LogInformation("{FileName} lontitud 0 (Err: 1)",
                            thrustFileName);
                            uploadResult.ErrorCode = 1;
                            uploadResult.ErrorMessage = $"{thrustFileName} vacio (Err:1)";
                        }
                        else if (file.Length > maxFileSize)
                        {
                            logger.LogInformation("{FileName} of {Length} bytes is " +
                            "larger than the limit of {Limit} bytes (Err: 2)",
                            thrustFileName, file.Length, maxFileSize);
                            uploadResult.ErrorCode = 2;
                            uploadResult.ErrorMessage = $"{thrustFileName} de {file.Length / 1000000} Mb es mayor a la capacidad permitida ({Math.Round((double)(maxFileSize / 1000000))}) Mb (Err:2)";
                        }
                        else
                        {
                            using var stream = new MemoryStream();
                            await file.CopyToAsync(stream);

                            ExcelPackage.LicenseContext = LicenseContext.Commercial;
                            ExcelPackage package = new();

                            package.Load(stream);
                            if (package.Workbook.Worksheets.Count > 0)
                            {
                                using (ExcelWorksheet ws = package.Workbook.Worksheets.First())
                                {
                                    for (int r = 2; r < (ws.Dimension.End.Row + 1); r++)
                                    {
                                        Persona persona = new();
                                        Empleado empleado = new();

                                        var row = ws.Cells[r, 1, r, ws.Dimension.End.Column].ToList();

                                        if (row.Count > 0)
                                        {
                                            if (ws.Cells[r, 1].Value is not null && ws.Cells[r, 2].Value is not null && ws.Cells[r, 3].Value is not null)
                                            {
                                                persona.Nombre = ws.Cells[r, 1].Value.ToString();
                                                persona.Ape_pat = ws.Cells[r, 2].Value.ToString();
                                                persona.Ape_mat = ws.Cells[r, 3].Value.ToString();
                                            }
                                            else
                                            {
                                                uploadResult.ErrorMessage = $"{thrustFileName} el nombre de la persona no puede estar vacio. Fila: {r} (Err: 11)";
                                                hasErrors = true; uploadResult.HasError = true; break;
                                            }

                                            if (ws.Cells[r, 4].Value is not null)
                                            {
                                                empleado.Noemp = ws.Cells[r, 4].Value.ToString();
                                                if (empleados.Any(x => x.Noemp == empleado.Noemp))
                                                {
                                                    var emp = empleados.First(x => x.Noemp == empleado.Noemp);
                                                    empleado.Idempleado = emp.Idempleado;
                                                    empleado.Idpersona = emp.Idpersona;
                                                }
                                            }
                                            else
                                            {
                                                uploadResult.ErrorMessage = $"{thrustFileName} el numero de empleado no puede estar vacio. Fila: {r} (Err: 11)";
                                                hasErrors = true; uploadResult.HasError = true; break;
                                            }

                                            if (ws.Cells[r, 5].Value is not null)
                                            {
                                                empleado.Division = ws.Cells[r, 5].Value.ToString();
                                            }
                                            else
                                            {
                                                uploadResult.ErrorMessage = $"{thrustFileName} la division no puede estar vacia. Fila: {r} (Err: 11)";
                                                hasErrors = true; uploadResult.HasError = true; break;
                                            }

                                            if (ws.Cells[r, 6].Value is not null)
                                            {
                                                if (!zonas.Any(x => x.Zona1.ToLower() == ws.Cells[r, 6].Value.ToString().ToLower()))
                                                {
                                                    uploadResult.ErrorMessage = $"{thrustFileName} la zona ingresada no existe. Zona: {ws.Cells[r, 6].Value} Fila: {r} (Err: 12)";
                                                    hasErrors = true; uploadResult.HasError = true; break;
                                                }
                                                else
                                                    empleado.ZonaId = zonas.First(x => x.Zona1.ToLower() == ws.Cells[r, 6].Value.ToString().ToLower()).ZonaId;
                                            }
                                            else
                                            {
                                                uploadResult.ErrorMessage = $"{thrustFileName} la zona no puede estar vacia. Fila: {r} (Err: 11)";
                                                hasErrors = true; uploadResult.HasError = true; break;
                                            }

                                            if (ws.Cells[r, 7].Value is not null)
                                            {
                                                if (!estaciones.Any(x => x.Nombre == ws.Cells[r, 7].Value.ToString()))
                                                {
                                                    uploadResult.ErrorMessage = $"{thrustFileName} la estacion ingresada no existe. Estacion: {ws.Cells[r, 7].Value} Fila: {r} (Err: 12)";
                                                    hasErrors = true; uploadResult.HasError = true; break;
                                                }
                                                else
                                                    empleado.Idestacion = estaciones.First(x => x.Nombre == ws.Cells[r, 7].Value.ToString() && x.Estatus == 1).Idestacion;
                                            }
                                            else
                                            {
                                                uploadResult.ErrorMessage = $"{thrustFileName} la estacion no puede estar vacia. Fila: {r} (Err: 11)";
                                                hasErrors = true; uploadResult.HasError = true; break;
                                            }

                                            if (ws.Cells[r, 8].Value is not null)
                                            {
                                                if (!departamentos.Any(x => x.Departamento1 == ws.Cells[r, 8].Value.ToString()))
                                                {
                                                    uploadResult.ErrorMessage = $"{thrustFileName} el departamento ingresado no existe. Departamento: {ws.Cells[r, 8].Value} Fila: {r} (Err: 12)";
                                                    hasErrors = true; uploadResult.HasError = true; break;
                                                }
                                                else
                                                    empleado.Iddepartamento = departamentos.First(x => x.Departamento1 == ws.Cells[r, 8].Value.ToString() && x.Idestatus == 1).Iddepartamento;
                                            }
                                            else
                                            {
                                                uploadResult.ErrorMessage = $"{thrustFileName} el departamento no puede estar vacio. Fila: {r} (Err: 11)";
                                                hasErrors = true; uploadResult.HasError = true; break;
                                            }

                                            if (ws.Cells[r, 9].Value is not null)
                                            {
                                                if (!puestos.Any(x => x.Puesto1 == ws.Cells[r, 9].Value.ToString()))
                                                {
                                                    uploadResult.ErrorMessage = $"{thrustFileName} el puesto ingresado no existe. Puesto: {ws.Cells[r, 9].Value} Fila: {r} (Err: 12)";
                                                    hasErrors = true; uploadResult.HasError = true; break;
                                                }
                                                else
                                                    empleado.Idpuesto = puestos.First(x => x.Puesto1 == ws.Cells[r, 9].Value.ToString()).Idpuesto;
                                            }
                                            else
                                            {
                                                uploadResult.ErrorMessage = $"{thrustFileName} el puesto no puede estar vacio. Fila: {r} (Err: 11)";
                                                hasErrors = true; uploadResult.HasError = true; break;
                                            }

                                            if (ws.Cells[r, 10].Value is not null && ws.Cells[r, 11].Value is not null)
                                            {
                                                if (!usernames.Any(x => x.UserName == ws.Cells[r, 10].Value.ToString()))
                                                {
                                                    if (!Validar_Contraseña(ws.Cells[r, 11].Value.ToString(), out Errors))
                                                    {
                                                        uploadResult.ErrorMessage = $"{thrustFileName} contraseña no valida. Error: {Errors} Fila: {r} (Err: 13)";
                                                        hasErrors = true; uploadResult.HasError = true; break;
                                                    }
                                                }
                                            }


                                            if (!hasErrors)
                                            {
                                                empleado.Idestatus = "1";

                                                if (empleado.Idempleado != 0)
                                                {
                                                    empleado.Fchactualizado = DateTime.Now;
                                                    context.Update(empleado);
                                                    context.Update(persona);
                                                    await context.SaveChangesAsync();
                                                }
                                                else
                                                {
                                                    empleado.Persona = persona;
                                                    empleado.Fchalta = DateTime.Now;
                                                    await context.AddAsync(empleado);
                                                    await context.SaveChangesAsync();
                                                }

                                                if (ws.Cells[r, 10].Value is not null && ws.Cells[r, 11].Value is not null)
                                                {
                                                    var user = new IdentityUsuario
                                                    {
                                                        UserName = ws.Cells[r, 10].Value.ToString(),
                                                        noemp = empleado.Noemp,
                                                        Idempleado = empleado.Idempleado,
                                                        Nombre = persona.Nombre,
                                                        ApellidoMaterno = persona.Ape_mat,
                                                        ApellidoPaterno = persona.Ape_pat,
                                                        ContraseñaTextoPlano = ws.Cells[r, 11].Value.ToString(),
                                                        Activo = true
                                                    };

                                                    if (await _userManager.FindByNameAsync(user.UserName) != null)
                                                    {
                                                        var result = await _userManager.UpdateAsync(user);
                                                        if (result.Succeeded)
                                                        {
                                                            var pass = usernames.First(x => x.UserName == ws.Cells[r, 10].Value.ToString());
                                                            if (pass.ContraseñaTextoPlano != user.ContraseñaTextoPlano)
                                                                await _userManager.ChangePasswordAsync(user, pass.ContraseñaTextoPlano, user.ContraseñaTextoPlano);
                                                        }
                                                    }
                                                    else
                                                    {

                                                        var result = await _userManager.CreateAsync(user, user.ContraseñaTextoPlano);
                                                        if (result.Succeeded)
                                                            await _userManager.AddToRoleAsync(user, "Usuario");
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                if (!hasErrors)
                                    uploadResult.Upload = true;
                            }
                        }

                        filesProcessed++;
                    }
                    catch (Exception e)
                    {
                        uploadResult.ErrorCode = 999;
                        uploadResult.ErrorMessage = $"(Error: {uploadResult.ErrorCode}: {e.Message})";
                    }
                }
                else
                {
                    logger.LogInformation("limite de archivos excedido. archivos {archivos}. limite de archivos {limite} (Err: 3)", file.Length, maxAllowedFiles);
                    uploadResult.ErrorCode = 2;
                    uploadResult.ErrorMessage = $"{thrustFileName} limite de archivos excedido. archivos {files.Count()}. limite de archivos {maxAllowedFiles} (Err: 3)";
                }
                uploadResults.Add(uploadResult);
            }
            return uploadResults;
        }


        [HttpPost("file/baja")]
        public async Task<ActionResult<IList<UploadResult>>> Subir_Excel_Empleados_Baja([FromForm] IEnumerable<IFormFile> files)
        {
            var maxAllowedFiles = 3;
            long maxFileSize = 1024 * 15 * 1024;
            var filesProcessed = 0;
            List<UploadResult> uploadResults = new();
            bool hasErrors = false;
            string Errors = string.Empty;

            foreach (var file in files)
            {
                List<Empleado> Empleados = new();

                var uploadResult = new UploadResult();

                var unthrustedFileName = file.FileName;
                uploadResult.FileName = unthrustedFileName;
                var thrustFileName = WebUtility.HtmlEncode(unthrustedFileName);

                if (filesProcessed < maxAllowedFiles)
                {
                    if (file.Length == 0)
                    {
                        logger.LogInformation("{FileName} lontitud 0 (Err: 1)",
                        thrustFileName);
                        uploadResult.ErrorCode = 1;
                        uploadResult.ErrorMessage = $"{thrustFileName} vacio (Err:1)";
                    }
                    else if (file.Length > maxFileSize)
                    {
                        logger.LogInformation("{FileName} of {Length} bytes is " +
                        "larger than the limit of {Limit} bytes (Err: 2)",
                        thrustFileName, file.Length, maxFileSize);
                        uploadResult.ErrorCode = 2;
                        uploadResult.ErrorMessage = $"{thrustFileName} de {file.Length / 1000000} Mb es mayor a la capacidad permitida ({Math.Round((double)(maxFileSize / 1000000))}) Mb (Err:2)";
                    }
                    else
                    {
                        using var stream = new MemoryStream();
                        await file.CopyToAsync(stream);

                        ExcelPackage.LicenseContext = LicenseContext.Commercial;
                        ExcelPackage package = new();

                        package.Load(stream);
                        if (package.Workbook.Worksheets.Count > 0)
                        {
                            using (ExcelWorksheet ws = package.Workbook.Worksheets.First())
                            {
                                hasErrors = false;
                                for (int r = 2; r < (ws.Dimension.End.Row + 1); r++)
                                {
                                    Persona persona = new();
                                    Empleado empleado = new();

                                    var row = ws.Cells[r, 1, r, ws.Dimension.End.Column].ToList();

                                    if (row.Count > 0)
                                    {
                                        if (ws.Cells[r, 1] != null)
                                        {
                                            if (context.Empleados.Any(x => x.Noemp.Equals(ws.Cells[r, 1].Value.ToString())))
                                            {
                                                empleado = context.Empleados.First(x => x.Noemp.Equals(ws.Cells[r, 1].Value.ToString()));

                                                empleado.Fchbaja = DateTime.Now;
                                                empleado.Idestatus = "2";

                                                context.Update(empleado);
                                                await context.SaveChangesAsync();

                                                if (context.Users.Any(x => x.Idempleado == empleado.Idempleado && x.Activo))
                                                {
                                                    var user = context.Users.FirstOrDefault(x => x.Idempleado == empleado.Idempleado);
                                                    user.Activo = false;
                                                    context.Update(user);
                                                    await context.SaveChangesAsync();
                                                }
                                            }
                                        }

                                    }
                                }
                            }

                            if (!hasErrors)
                                uploadResult.Upload = true;
                        }
                    }

                    filesProcessed++;
                }
                else
                {
                    logger.LogInformation("limite de archivos excedido. archivos {archivos}. limite de archivos {limite} (Err: 3)", file.Length, maxAllowedFiles);
                    uploadResult.ErrorCode = 2;
                    uploadResult.ErrorMessage = $"{thrustFileName} limite de archivos excedido. archivos {files.Count()}. limite de archivos {maxAllowedFiles} (Err: 3)";
                }
                uploadResults.Add(uploadResult);
            }
            return uploadResults;
        }

        [HttpDelete("baja/{noemp}")]
        public async Task<ActionResult<EmpleadoPersonaDTO>> Dar_Baja_Empleado(string noemp)
        {
            try
            {
                if (string.IsNullOrEmpty(noemp) || string.IsNullOrWhiteSpace(noemp))
                    return NotFound();

                var empleado = context.Empleados.FirstOrDefault(x => x.Noemp.Equals(noemp));

                empleado.Fchbaja = DateTime.Now;
                empleado.Idestatus = "2";

                context.Update(empleado);
                await context.SaveChangesAsync();

                if (context.Users.Any(x => x.Idempleado == empleado.Idempleado && x.Activo))
                {
                    var user = context.Users.FirstOrDefault(x => x.Idempleado == empleado.Idempleado);
                    user.Activo = false;
                    context.Update(user);
                    await context.SaveChangesAsync();
                }

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        private bool Validar_Contraseña(string password, out string Error)
        {
            Error = string.Empty;
            var tiene_numeros = new Regex(@"[0-9]+");
            var tiene_mayusculas = new Regex(@"[A-Z]+");
            var tiene_logitud_min_max = new Regex(@".{6,50}");
            var tiene_simbolos = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");

            if (!tiene_numeros.IsMatch(password))
            {
                Error = "Debe de contener numeros. minimo: 1";
                return false;
            }

            else if (!tiene_mayusculas.IsMatch(password))
            {
                Error = "Debe de contener mayusculas. minimo: 1";
                return false;
            }

            else if (!tiene_logitud_min_max.IsMatch(password))
            {
                Error = "Debe de contener al menos 6 caractes y maximo 50";
                return false;
            }

            else if (!tiene_simbolos.IsMatch(password))
            {
                Error = "Debe de contener caracteres especiales. minimo: 1";
                return false;
            }
            else if (!(password.Distinct().Count() > 0))
            {
                Error = "Debe de contener caracteres unicos: minimo 1";
                return false;
            }
            else
                return true;
        }

        #region EmpleadoconLogger

        //[HttpGet]
        //public async Task<ActionResult<List<Empleado>>> Get()
        //{
        //    logger.LogWarning("Log al pedir empleados");

        //    //ejemplo con la excepcion incluida
        //    try
        //    {
        //        throw new NotImplementedException();
        //    }
        //    catch (NotImplementedException ex)
        //    {
        //        logger.LogError(ex, ex.Message);
        //    }

        //    var user = await _userManager.GetUserAsync(HttpContext.User);
        //    logger.LogError("{UserId} some message", user.Id);

        //    var listadodeempleados = await context.Empleados.ToListAsync();
        //    return listadodeempleados;
        //}

        #endregion

        #region GetEmpleadosinFiltros

        public class ParametrosBusqueda
        {
            public int Pagina { get; set; } = 1;
            public int CantidadRegistros { get; set; } = 12;
            public PaginacionDTO Paginacion
            {
                get { return new PaginacionDTO() { Pagina = Pagina, CantidadRegistros = CantidadRegistros }; }
            }
            public int EmpleadoId { get; set; }
            public int DepartamentoId { get; set; }
            public string Division { get; set; }
            public bool Activo { get; set; }

            public int zona { get; set; }
            public string Nombre { get; set; } = "";

        }

        //[HttpGet]
        //public async Task<ActionResult<List<Empleado>>> GetEmp([FromQuery] PaginacionDTO paginacion)
        //{    
        //    //para obtener una union de dos tablas que no estan unidas por fk, se realiza una consulta linq
        //    //se crea un nuevo objeto empleado pero se tiene que poner toda su data y como propiedad de navegacion
        //    //se hace otra consulta para obtener la data persona por que no se puede usar el include
        //    List<Empleado> empleados = (from e in context.Empleados
        //                               select new Empleado
        //                               {
        //                                   Idempleado = e.Idempleado,
        //                                   Idpersona = e.Idpersona,
        //                                   Img = e.Img,
        //                                   Noemp = e.Noemp,
        //                                   Correo = e.Correo,
        //                                   Telefono = e.Telefono,
        //                                   Iddepartamento = e.Iddepartamento,
        //                                   Idarea = e.Idarea,
        //                                   Idpuesto = e.Idpuesto,
        //                                   Nomina = e.Nomina,
        //                                   Variable = e.Variable,
        //                                   Idtipo = e.Idtipo,
        //                                   Fchalta = e.Fchalta,
        //                                   Fchbaja = e.Fchbaja,
        //                                   Persona = context.Personas.Where(x => x.Idpersona == e.Idpersona).FirstOrDefault(),
        //                                   Departamento = context.Departamentos.Where(x => x.Iddepartamento == e.Iddepartamento).FirstOrDefault(),
        //                                   Area = context.Areas.Where(x => x.Idarea == e.Idarea).FirstOrDefault(),
        //                                   Puesto = context.Puestos.Where(x => x.Idpuesto == e.Idpuesto).FirstOrDefault()
        //                               }).ToList();

        //    foreach (var item in empleados)
        //    {
        //        if (string.IsNullOrEmpty(item.Img))
        //        {
        //            // Aquí colocas la URL de la imagen por defecto
        //            item.Img = "Img" + "/" + "usuario.png";
        //        }
        //    }

        //    var queryable = empleados.AsQueryable();
        //    await HttpContext.InsertarParametrosPaginacionEnRespuesta(queryable, paginacion.CantidadRegistros,true);
        //    return queryable.Paginar(paginacion).ToList();
        //}

        #endregion

        [HttpGet("excel/formato")]
        public ActionResult Obtener_Formato()
        {
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            var excel = new ExcelPackage();

            var ws_empleados = excel.Workbook.Worksheets.Add("Empleados");

            ws_empleados.Cells["A1"].LoadFromCollection(new List<PersonaEmpleadoDTO>(), x => { x.PrintHeaders = true; x.TableStyle = TableStyles.Medium2; });
            ws_empleados.Cells["J2"].Formula = "SI(ISTEXT(M2),SI(ISTEXT(D2),CONCATENATE(D2,MID(MAYUSC(M2),1,2)),\"\"),\"\")";
            ws_empleados.Cells["K2"].Formula = "CONCATENATE(\"S1msa*\",D2)";

            ws_empleados.Cells[1, 1, ws_empleados.Dimension.End.Row, ws_empleados.Dimension.End.Column].AutoFitColumns();
            return Ok(excel.GetAsByteArray());
        }

        [HttpGet("excel/formato/baja")]
        public ActionResult Obtener_Formato_Baja()
        {
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            var excel = new ExcelPackage();

            var ws_empleados = excel.Workbook.Worksheets.Add("Empleados");

            List<ExpandoObject> Nos_Empleado = new();
            dynamic empleado = new ExpandoObject();
            empleado.NoEmplado = string.Empty;

            Nos_Empleado.Add(empleado);

            ws_empleados.Cells["A1"].LoadFromCollection(Nos_Empleado, x => { x.PrintHeaders = true; x.TableStyle = TableStyles.Medium2; });

            ws_empleados.Cells["A2"].Style.Numberformat.Format = "@";

            ws_empleados.Cells[1, 1, ws_empleados.Dimension.End.Row, ws_empleados.Dimension.End.Column].AutoFitColumns();
            return Ok(excel.GetAsByteArray());
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<EmpleadoPersonaDTO>> Get(int id)
        {
            Empleado empleado = (from e in context.Empleados
                                 where e.Idempleado == id
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
                                     Division = e.Division,
                                     ImagenesCumple = context.ImagenesCumpleEmpleado.Where(x => x.EmpleadoId == e.Idempleado).ToList(),
                                     Departamento = context.Departamentos.Where(x => x.Iddepartamento == e.Iddepartamento).FirstOrDefault(),
                                     Area = context.Areas.Where(x => x.Idarea == e.Idarea).FirstOrDefault(),
                                     Puesto = context.Puestos.Where(x => x.Idpuesto == e.Idpuesto).FirstOrDefault()
                                 })
                                        .FirstOrDefault();

            if (empleado == null) { return NotFound(); }

            //si el usuario no subio imagen poner una por defecto
            if (string.IsNullOrEmpty(empleado.Img))
            {
                // Aquí colocas la URL de la imagen por defecto
                empleado.Img = "Img" + "/" + "avatars-1.png";
            }

            var persona = await context.Personas.Where(x => x.Idpersona == empleado.Idpersona).FirstOrDefaultAsync();
            if (persona == null) { return NotFound(); }

            var model = new EmpleadoPersonaDTO();
            model.Empleado = empleado;
            model.Persona = persona;

            return model;
        }

        //utilizamos fromquery para traer los parametros de busqueda
        [HttpGet("filtrar")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Empleado>>> Get([FromQuery] ParametrosBusqueda parametrosBusqueda)
        {
            bool mostrar = true;

            //para obtener una union de dos tablas que no estan unidas por fk, se realiza una consulta linq
            //se crea un nuevo objeto empleado pero se tiene que poner toda su data y como propiedad de navegacion
            //se hace otra consulta para obtener la data persona por que no se puede usar el include
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
                                            Division = e.Division,
                                            ZonaId = e.ZonaId,
                                            Persona = context.Personas.Where(x => x.Idpersona == e.Idpersona).FirstOrDefault(),
                                            Departamento = context.Departamentos.Where(x => x.Iddepartamento == e.Iddepartamento).FirstOrDefault(),
                                            Area = context.Areas.Where(x => x.Idarea == e.Idarea).FirstOrDefault(),
                                            Puesto = context.Puestos.Where(x => x.Idpuesto == e.Idpuesto).FirstOrDefault(),
                                        }).ToList();

            var queryable = empleados.OrderBy(x => x.Idpersona).AsQueryable();

            if (parametrosBusqueda.EmpleadoId != 0)
            {
                queryable = queryable.Where(x => x.Idempleado == parametrosBusqueda.EmpleadoId);
            }
            if (parametrosBusqueda.DepartamentoId != 0)
            {
                queryable = queryable.Where(x => x.Iddepartamento == parametrosBusqueda.DepartamentoId);
            }
            if (parametrosBusqueda.zona != 0)
            {
                queryable = queryable.Where(x => x.ZonaId == parametrosBusqueda.zona);
            }
            if (!string.IsNullOrEmpty(parametrosBusqueda.Division))
            {
                queryable = queryable.Where(x => x.Division != null && x.Division.ToLower().Contains(parametrosBusqueda.Division.ToLower()));
            }
            //if (parametrosBusqueda.Activo == true)
            //{
            //    mostrar = false;
            //    //queryable = queryable.Where(x => x.Activo == mostrar);               
            //}

            //si el usuario no subio imagen poner una por defecto
            foreach (var item in queryable)
            {
                if (string.IsNullOrEmpty(item.Img))
                {
                    // Aquí colocas la URL de la imagen por defecto
                    item.Img = "Img" + "/" + "usuario.png";
                }
            }
            //paginacion
            await HttpContext.InsertarParametrosPaginacionEnRespuesta(queryable, parametrosBusqueda.CantidadRegistros, true);
            var listaARetornar = queryable.Paginar(parametrosBusqueda.Paginacion).ToList();
            return listaARetornar;
        }

        //buscar empleados para filtro
        [HttpGet("buscar/{textoBusqueda}")]
        public async Task<ActionResult<List<Empleado>>> GetEmpleado(string textoBusqueda)
        {
            if (textoBusqueda.Length > 3)
            {
                if (string.IsNullOrWhiteSpace(textoBusqueda)) { return new List<Empleado>(); }
                textoBusqueda = textoBusqueda.ToLower();

                List<Empleado> empleados = await (from e in context.Empleados
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
                                                      Division = e.Division,
                                                      Persona = context.Personas.Where(x => x.Idpersona == e.Idpersona).FirstOrDefault(),
                                                  })
                                            .Where(x => x.Persona.Nombre.ToLower().Contains(textoBusqueda) || x.Persona.Ape_pat.ToLower().Contains(textoBusqueda) || x.Noemp.ToLower().Contains(textoBusqueda))
                                            .ToListAsync();

                return empleados;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(textoBusqueda)) { return new List<Empleado>(); }
                textoBusqueda = textoBusqueda.ToLower();

                List<Empleado> empleados = await (from e in context.Empleados
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
                                                      Division = e.Division,
                                                      Persona = context.Personas.Where(x => x.Idpersona == e.Idpersona).FirstOrDefault(),
                                                  })
                                           .Where(x => x.Persona.Nombre.ToLower().Contains(textoBusqueda) || x.Persona.Ape_pat.ToLower().Contains(textoBusqueda) || x.Noemp.ToLower().Contains(textoBusqueda))
                                           .Take(50)
                                           .ToListAsync();

                return empleados;
            }
        }

        //buscar empleados para filtro
        [HttpGet("buscar/nombre/{nombre}")]
        public async Task<ActionResult<List<Empleado>>> GetEmpleadoPorNombre(string nombre)
        {
            if (nombre.Length > 3)
            {
                if (string.IsNullOrWhiteSpace(nombre)) { return new List<Empleado>(); }
                nombre = nombre.ToLower();

                List<Empleado> empleados = await (from e in context.Empleados
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
                                                      Division = e.Division,
                                                      Persona = context.Personas.Where(x => x.Idpersona == e.Idpersona).FirstOrDefault(),
                                                  })
                                            .Where(x => x.Persona.Nombre.ToLower().Contains(nombre))
                                            .ToListAsync();

                return empleados;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(nombre)) { return new List<Empleado>(); }
                nombre = nombre.ToLower();

                List<Empleado> empleados = await (from e in context.Empleados
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
                                                      Division = e.Division,
                                                      Persona = context.Personas.Where(x => x.Idpersona == e.Idpersona).FirstOrDefault(),
                                                  })
                                           .Where(x => x.Persona.Nombre.ToLower().Contains(nombre))
                                           .Take(50)
                                           .ToListAsync();

                return empleados;
            }
        }

        //buscar empleados para filtro
        [HttpGet("buscar/noemp/{noemp}")]
        public async Task<ActionResult<List<Empleado>>> GetEmpleadoPorNoemp(string noemp)
        {
            if (noemp.Length > 3)
            {
                if (string.IsNullOrWhiteSpace(noemp)) { return new List<Empleado>(); }
                noemp = noemp.ToLower();

                List<Empleado> empleados = await (from e in context.Empleados
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
                                                      Division = e.Division,
                                                      Persona = context.Personas.Where(x => x.Idpersona == e.Idpersona).FirstOrDefault(),
                                                  })
                                            .Where(x => x.Noemp.ToLower().Contains(noemp))
                                            .ToListAsync();

                return empleados;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(noemp)) { return new List<Empleado>(); }
                noemp = noemp.ToLower();

                List<Empleado> empleados = await (from e in context.Empleados
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
                                                      Division = e.Division,
                                                      Persona = context.Personas.Where(x => x.Idpersona == e.Idpersona).FirstOrDefault(),
                                                  })
                                           .Where(x => x.Noemp.ToLower().Contains(noemp))
                                           .Take(50)
                                           .ToListAsync();

                return empleados;
            }
        }

        //buscar empleados para filtro
        [HttpGet("buscar/apepat/{apellido}")]
        public async Task<ActionResult<List<Empleado>>> GetEmpleadosApellidoPat(string apellido)
        {
            if (apellido.Length > 3)
            {
                if (string.IsNullOrWhiteSpace(apellido)) { return new List<Empleado>(); }
                apellido = apellido.ToLower();

                List<Empleado> empleados = await (from e in context.Empleados
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
                                                      Division = e.Division,
                                                      Persona = context.Personas.Where(x => x.Idpersona == e.Idpersona).FirstOrDefault(),
                                                  })
                                            .Where(x => x.Persona.Ape_pat.ToLower().Contains(apellido))
                                            .ToListAsync();

                return empleados;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(apellido)) { return new List<Empleado>(); }
                apellido = apellido.ToLower();

                List<Empleado> empleados = await (from e in context.Empleados
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
                                                      Division = e.Division,
                                                      Persona = context.Personas.Where(x => x.Idpersona == e.Idpersona).FirstOrDefault(),
                                                  })
                                           .Where(x => x.Persona.Ape_pat.ToLower().Contains(apellido))
                                           .Take(50)
                                           .ToListAsync();

                return empleados;
            }
        }

        //buscar empleados para filtro
        [HttpGet("buscar/apemat/{apellido}")]
        public async Task<ActionResult<List<Empleado>>> GetEmpleadosApellidoMat(string apellido)
        {
            if (apellido.Length > 3)
            {
                if (string.IsNullOrWhiteSpace(apellido)) { return new List<Empleado>(); }
                apellido = apellido.ToLower();

                List<Empleado> empleados = await (from e in context.Empleados
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
                                                      Division = e.Division,
                                                      Persona = context.Personas.Where(x => x.Idpersona == e.Idpersona).FirstOrDefault(),
                                                  })
                                            .Where(x => x.Persona.Ape_mat.ToLower().Contains(apellido))
                                            .ToListAsync();

                return empleados;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(apellido)) { return new List<Empleado>(); }
                apellido = apellido.ToLower();

                List<Empleado> empleados = await (from e in context.Empleados
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
                                                      Division = e.Division,
                                                      Persona = context.Personas.Where(x => x.Idpersona == e.Idpersona).FirstOrDefault(),
                                                  })
                                           .Where(x => x.Persona.Ape_mat.ToLower().Contains(apellido))
                                           .Take(50)
                                           .ToListAsync();

                return empleados;
            }
        }

        [Route("ImgCumpleEmpleado")]
        [HttpPost]
        public async Task<ActionResult<int>> Post(Empleado empleado)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            List<ImagenesCumpleEmpleado> listadeimagenes = new List<ImagenesCumpleEmpleado>();

            //recorremos la lsita de imagenes que trae la data del emmpleado, y por cada iteracion se crea un objeto 
            //ImagenesCumpleEmpleado y le asignamos el idempleado ya que el que viene en la data no lo trae
            //aqui es donde hacemos la relacion empleado-imagenescumpleempleado
            foreach (var item in empleado.ImagenesCumple)
            {
                ImagenesCumpleEmpleado imagen = new ImagenesCumpleEmpleado();
                imagen.EmpleadoId = empleado.Idempleado;
                imagen.Imagen = item.Imagen;

                listadeimagenes.Add(imagen);
            }

            context.ImagenesCumpleEmpleado.AddRange(listadeimagenes);
            await context.SaveChangesAsync(user.Id);
            return empleado.Idempleado;
        }
    }
}
