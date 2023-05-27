using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnePlace.Server.Data;
using OnePlace.Server.Helpers;
using OnePlace.Shared.Entidades.SimsaCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OnePlace.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrador")]
    public class DepartamentoController : ControllerBase
    {
        private readonly oneplaceContext context;
        private readonly UserManager<ApplicationUser> _userManager;
        public DepartamentoController(oneplaceContext context, UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            _userManager = userManager;
        }

        //buscar deptos para filtro
        [HttpGet("buscar/{textoBusqueda}")]
        public async Task<ActionResult<List<Departamento>>> GetMarc(string textoBusqueda)
        {
            if (textoBusqueda.Length > 3)
            {
                if (string.IsNullOrWhiteSpace(textoBusqueda)) { return new List<Departamento>(); }
                textoBusqueda = textoBusqueda.ToLower();
                return await context.Departamentos.Where(x => x.Departamento1.ToLower().Contains(textoBusqueda)).ToListAsync();
            }
            else
            {
                if (string.IsNullOrWhiteSpace(textoBusqueda)) { return new List<Departamento>(); }
                textoBusqueda = textoBusqueda.ToLower();
                return await context.Departamentos.Where(x => x.Departamento1.ToLower().Contains(textoBusqueda)).Take(50).ToListAsync();
            }
        }
        //busca los departamenros por la razon social
        [HttpGet("{razonId:int}")]
        public async Task<ActionResult<List<Departamento>>> GetDepartamentoByRazonId(int razonId)
        {
            var departamentos = await context.area_departamento_empresa
                .Where( x => x.Idempresa == razonId)
                .Include( x => x.Departamento)
                .ToListAsync();
            //Variable creada para hacer un disctinct de departamentos para no traer departamentos repetidos
            var departamentosEnum = departamentos.DistinctBy(x => x.Departamento.Departamento1);
            return Ok(departamentosEnum.ToList());
        }

        //buscar razon social para filtro
        [HttpGet("razon/buscar/{textoBusqueda}")]
        public async Task<ActionResult<List<Empresa>>> GetRazon(string textoBusqueda)
        {
            if (textoBusqueda.Length > 3)
            {
                if (string.IsNullOrWhiteSpace(textoBusqueda)) { return new List<Empresa>(); }
                textoBusqueda = textoBusqueda.ToLower();
                return await context.Empresas.Where(x => x.Razonsocial.ToLower().Contains(textoBusqueda)).ToListAsync();
            }
            else
            {
                if (string.IsNullOrWhiteSpace(textoBusqueda)) { return new List<Empresa>(); }
                textoBusqueda = textoBusqueda.ToLower();
                return await context.Empresas.Where(x => x.Razonsocial.ToLower().Contains(textoBusqueda)).Take(50).ToListAsync();
            }
        }
    }
}
