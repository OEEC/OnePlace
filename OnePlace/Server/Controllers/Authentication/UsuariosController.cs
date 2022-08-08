using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnePlace.Server.Data;
using OnePlace.Server.Helpers;
using OnePlace.Shared.DTOs;
using System;
using System.Collections.Generic;
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
        [HttpGet]       
        public async Task<ActionResult<List<UsuarioDTO>>> Get([FromQuery] PaginacionDTO paginacion)
        {
            var queryable = context.Users.Where(x => x.Activo == true).AsQueryable();
            await HttpContext.InsertarParametrosPaginacionEnRespuesta(queryable, paginacion.CantidadRegistros);
            return await queryable.Paginar(paginacion)
            .Select(x => new UsuarioDTO { NumeroEmpleado = x.UserName, UserId = x.Id, Contraseña = x.ContraseñaTextoPlano, Nombre = x.Nombre, Apellido_Paterno = x.ApellidoPaterno}).ToListAsync();//hacemos un mapeo hacia usuariodto          
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
    }
}
