using Dapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnePlace.Server.Data;
using OnePlace.Server.Helpers;
using OnePlace.Shared.DTOs;
using OnePlace.Shared.Entidades.SimsaCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using static OnePlace.Server.Data.ApplicationUser;

namespace OnePlace.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrador")]
    public class UsuariosController : ControllerBase
    {
        private readonly oneplaceContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        //utilizamos un constructor para poder inyectar una instancia de applicationdbcontext y otras
        public UsuariosController(oneplaceContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            this.context = context;//lo ponemos como un campo
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        //con esta peticion traemos un listado de usuarios desde la bd
        //[HttpGet]       
        //public async Task<ActionResult<List<UsuarioDTO>>> Get([FromQuery] PaginacionDTO paginacion)
        //{
        //    var queryable = context.Users.Where(x => x.Activo == true).AsQueryable();
        //    await HttpContext.InsertarParametrosPaginacionEnRespuesta(queryable, paginacion.CantidadRegistros);
        //    return await queryable.Paginar(paginacion)
        //    .Select(x => new UsuarioDTO { NumeroEmpleado = x.UserName, UserId = x.Id, Contraseña = x.ContraseñaTextoPlano, Nombre = x.Nombre, Apellido_Paterno = x.ApellidoPaterno}).ToListAsync();//hacemos un mapeo hacia usuariodto          
        //}

        //utilizamos fromquery para traer los parametros de busqueda
        [HttpGet("filtrar")]
        [AllowAnonymous]
        public async Task<ActionResult<PaginadorGenerico<UsuarioDTO>>> Ge([FromQuery] ParametrosBusqueda parametrosBusqueda)
        {
            PaginadorGenerico<UsuarioDTO> _PaginadorConceptos;

            List<UsuarioDTO> listaARetornar = new List<UsuarioDTO>();//listado con filtrado de registros
            List<UsuarioDTO> listaARetornarExport = new List<UsuarioDTO>();//listado sin filtrado de registros

            List<ApplicationUser> listadeusuarios = new List<ApplicationUser>();//listado con filtrado por estacion
            var queryable = context.Users.Where(x => x.Activo == true).AsQueryable();//listado inicial queryable

            //si viene un id de estacion entra al if
            if (parametrosBusqueda.EstacionId != 0)
            {
                //traemos una lista de empleados que pertenezcan a una estacion, por medio de idestacion se realiza el filtro
                var listadeempleados = await context.Empleados.Where(x => x.Idestacion == parametrosBusqueda.EstacionId).ToListAsync();

                foreach (var item in listadeempleados)
                {
                    //por cada empleado obtenemos un usuario para guardarlo en la lista de usuarios por estacion
                    var usuario = await context.Users.Where(x => x.Idempleado == item.Idempleado && x.Activo == true).FirstOrDefaultAsync();

                    if (usuario != null)
                    {
                        listadeusuarios.Add(usuario);
                    }
                }

                //actualizamos la lista inicial de usuarios por la lista de usuarios filtrado por estacion
                queryable = listadeusuarios.AsQueryable();

                #region LISTINMEMORY LISTUSER

                ////aqui se usaba si la lista se creaba en memoria se pone la variable true para poder usar tolist en ligar de tolistasyn
                //await HttpContext.InsertarParametrosPaginacionEnRespuesta(queryable, parametrosBusqueda.CantidadRegistros, true);
                //return queryable.Paginar(parametrosBusqueda.Paginacion)
                //.Select(x => new UsuarioDTO
                //{
                //    NumeroEmpleado = x.UserName,
                //    UserId = x.Id,
                //    Contraseña = x.ContraseñaTextoPlano,
                //    Nombre = x.Nombre,
                //    Apellido_Paterno = x.ApellidoPaterno
                //})
                //.ToList();//hacemos un mapeo hacia usuariodto

                #endregion
            }

            //se asignan los registros a los listados despues de cada query para que se vean reflejados los filtros
            listaARetornar = queryable.Select(x => new UsuarioDTO
            {
                NumeroEmpleado = x.UserName,
                UserId = x.Id,
                Contraseña = x.ContraseñaTextoPlano,
                Nombre = x.Nombre,
                Apellido_Paterno = x.ApellidoPaterno,
                Apellido_Materno = x.ApellidoMaterno
            })
            .ToList();//hacemos un mapeo hacia usuariodto    

            //la lista a exportar es igual a la lista final pero sin paginacion
            listaARetornarExport = listaARetornar;

            #region PAGINACION          

            int _TotalRegistros = 0;
            int _TotalPaginas = 0;

            // Número total de registros de la coleccion 
            _TotalRegistros = listaARetornar.Count();

            // Obtenemos la 'página de registros' de la coleccion 
            listaARetornar = listaARetornar.Skip((parametrosBusqueda.Pagina - 1) * parametrosBusqueda.CantidadRegistros)
                                                             .Take(parametrosBusqueda.CantidadRegistros)
                                                             .ToList();
            // Número total de páginas de la coleccion
            _TotalPaginas = (int)Math.Ceiling((double)_TotalRegistros / parametrosBusqueda.CantidadRegistros);

            //Instanciamos la 'Clase de paginación' y asignamos los nuevos valores
            _PaginadorConceptos = new PaginadorGenerico<UsuarioDTO>()
            {
                RegistrosPorPagina = parametrosBusqueda.CantidadRegistros,
                TotalRegistros = _TotalRegistros,
                TotalPaginas = _TotalPaginas,
                PaginaActual = parametrosBusqueda.Pagina,
                Resultado = listaARetornar,
                ResultadoAExportar = listaARetornarExport
            };
            return _PaginadorConceptos;

            #endregion

            #region DBCONTEXT LISTUSER

            ////aqui se usaba para el query normal de usuarios cuando venia del dbcontext
            //await HttpContext.InsertarParametrosPaginacionEnRespuesta(queryable, parametrosBusqueda.CantidadRegistros);
            //return await queryable.Paginar(parametrosBusqueda.Paginacion)
            //.Select(x => new UsuarioDTO
            //{
            //    NumeroEmpleado = x.UserName,
            //    UserId = x.Id,
            //    Contraseña = x.ContraseñaTextoPlano,
            //    Nombre = x.Nombre,
            //    Apellido_Paterno = x.ApellidoPaterno
            //})
            //.ToListAsync();//hacemos un mapeo hacia usuariodto

            #endregion
        }
        public class ParametrosBusqueda
        {
            public int Pagina { get; set; } = 1;
            public int CantidadRegistros { get; set; } = 10;
            public PaginacionDTO Paginacion
            {
                get { return new PaginacionDTO() { Pagina = Pagina, CantidadRegistros = CantidadRegistros }; }
            }
            public int EstacionId { get; set; }
        }

        //con esta peticion traemos el listado de roles desde la bd
        [HttpGet("roles")]
        public async Task<ActionResult<List<RolDTO>>> Get()
        {
            //mapeamos hacia rolesdto donde el rolid es igual a id
            return await context.Roles.Select(x => new RolDTO { Nombre = x.Name, RoleId = x.Id }).ToListAsync();
        }

        //creamos las peticiones para asignar y eliminar un rol en el httpost le pasamos los metodos creados en editarusuario.razor
        [HttpPost("asignarRol")]
        public async Task<ActionResult> AsignarRolUsuario(EditarRolDTO editarRolDTO)
        {
            //lo buscamos por su id con findbyid
            var usuario = await userManager.FindByIdAsync(editarRolDTO.UserId);

            //buscamos el usuario en la tabla aspnetuserrol 
            var existe = await context.UserRoles.AnyAsync(x => x.UserId == usuario.Id);

            //sino exite el usuario en la tabla aspnetuserrol lo agregamos
            if (!existe)
            {
                await userManager.AddToRoleAsync(usuario, editarRolDTO.RoleId);//agregamos un rol a un usuario

                //esto es para ponerle el tipousuario al empleado en base al rol seleccionado
                await AsignarTipodeUsuario(editarRolDTO);
            }
            else
            {
                //si exite el usuario lo borramos directamente de la base de datos 
                await context.Database.ExecuteSqlInterpolatedAsync($"DELETE FROM AspNetUserRoles WHERE UserId = {usuario.Id};");

                //y volvemos asignar el nuevo rol seleccionado
                await userManager.AddToRoleAsync(usuario, editarRolDTO.RoleId);

                //esto es para ponerle el tipousuario al empleado en base al rol seleccionado
                await AsignarTipodeUsuario(editarRolDTO);
            }

            return NoContent();
        }

        [HttpPost("removerRol")]
        public async Task<ActionResult> RemoverUsuarioRol(EditarRolDTO editarRolDTO)
        {
            var usuario = await userManager.FindByIdAsync(editarRolDTO.UserId);
            await userManager.RemoveFromRoleAsync(usuario, editarRolDTO.RoleId);//eliminamos un rol de un usuario
            return NoContent();
        }

        ////creamos una paticion delete para borrar los registros
        //[HttpDelete("{Id}")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrador")]
        ////aqui no pasamos un objeto sino un string id no es como el ejemplo de arriba que asaban editarroldto
        //public async Task<ActionResult> Delete(string Id)
        //{
        //    //el id se puede buscar sin necesidad de hacer UsuarioDTO.UserId simplemente poner id que es lo que se pasa como parametro
        //    var usuario = await userManager.FindByIdAsync(Id);
        //    //el borrar usuario borra un objeto de tipo usuario
        //    await userManager.DeleteAsync(usuario);
        //    return NoContent();
        //}

        [Route("Desactivar")]
        [HttpPut]
        public async Task<ActionResult> PutDesactivar(UsuarioDTO usuarioDTO)
        {
            var user = await userManager.GetUserAsync(HttpContext.User);

            //obtener el registro original usando el método FindAsync 
            var oldUsuario = await context.Users.FindAsync(usuarioDTO.UserId);

            //las propiedades sin cambios se ignoran y solo los valores de cambios se incluyen en la consulta de actualización
            context.Entry(oldUsuario).CurrentValues.SetValues(usuarioDTO);

            await context.SaveChangesAsync(user.Id);
            return NoContent();
        }
        public async Task AsignarTipodeUsuario(EditarRolDTO editarRolDTO)
        {
            //lo buscamos por su id con findbyid
            var usuario = await userManager.FindByIdAsync(editarRolDTO.UserId);

            if (editarRolDTO.RoleId == "Administrador")
            {
                //se tuvo que poner un tostring para poder mapearlo contra la bd de mysql
                usuario.TipodeUsuarios = TipodeUsuario.Administrador.ToString();
            }
            else
            {
                usuario.TipodeUsuarios = TipodeUsuario.Usuario.ToString();
            }

            //obtener el registro original usando el método FindAsync 
            var oldusuario = await context.Users.FindAsync(usuario.Id);

            //las propiedades sin cambios se ignoran y solo los valores de cambios se incluyen en la consulta de actualización
            context.Entry(oldusuario).CurrentValues.SetValues(usuario);

            await context.SaveChangesAsync();
        }
        //Crear Usuario en base a los empleados en la base de datos
        [HttpPost("crear")]
        public async Task<ActionResult<List<Empleado>>> CreeateNewUsers(Empleado empleado)
        {
            try { 
                List<Empleado> empleados = (from e in context.Empleados
                                            where e.Fchbaja == null
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
                                                Persona = context.Personas.Where(x => x.Idpersona == e.Idpersona).FirstOrDefault()
                                            }).ToList();
                //Recorre la lista de empleados
                foreach (var item in empleados)
                {
                    //Si el numero de empleado no es nulo crea al empleado
                    if (!string.IsNullOrEmpty(item.Noemp)) 
                    {
                        var inicialesZona = "";
                        var zona = context.Zonas.FirstOrDefault(x => x.ZonaId == item.ZonaId);
                        //Si no tiene zona asignada le añade un NA de lo contrario separa sus dos primeros letras en mayusculas
                        if (zona is not null)
                        {
                            inicialesZona = zona.Zona1.ToUpper().Substring(0,2);
                        }
                        else 
                        {
                            inicialesZona = "NA";
                        }
                        //Asigna valores a objeto usuario
                        var user = new ApplicationUser
                        {
                            UserName = item.Noemp.Trim() + inicialesZona,
                            noemp = item.Noemp,
                            Idempleado = item.Idempleado,
                            Nombre = item.Persona.Nombre,
                            ApellidoMaterno = item.Persona.ApeMat,
                            ApellidoPaterno = item.Persona.ApePat,
                            //Empleado = null,
                            ContraseñaTextoPlano = $"S1msa*{item.Noemp}",
                            Activo = true
                        };
                        var existUser = context.Users.FirstOrDefault(x => x.UserName == user.UserName);
                        Debug.WriteLine(existUser);
                        //Si no existe ya el numero de empleado del empleado
                        if (existUser is null)
                        {
                            Debug.WriteLine(existUser);
                            var result = await userManager.CreateAsync(user, user.ContraseñaTextoPlano);

                            if (result.Succeeded)
                            {
                                await userManager.AddToRoleAsync(user, "Usuario");
                            }
                            else
                            {
                                return BadRequest(result.Errors);
                            }
                        }
                    }
                }
                return Ok();
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }

    }
}
