using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using OnePlace.Server.Excepciones;
using OnePlace.Shared.IdentityModels;
using System.Threading.Tasks;

namespace OnePlace.Server.Helpers
{
    public class UsuarioHelper : IUsuarioHelper
    {
        private readonly UserManager<IdentityUsuario> userManager;
        private readonly IHttpContextAccessor httpContext;

        public UsuarioHelper(UserManager<IdentityUsuario> userManager, IHttpContextAccessor httpContext)
        {
            this.userManager = userManager;
            this.httpContext = httpContext;
        }

        public async Task<IdentityUsuario> GetUsuario()
        {
            var usuario = await userManager.FindByNameAsync(httpContext.HttpContext.User.Identity.Name);
            return usuario is null ? throw new UsuarioException("Usuario no valido") : usuario;
        }

        public async Task<string> GetUsuarioId()
        {
            var usuario = await userManager.FindByNameAsync(httpContext.HttpContext.User.Identity.Name);
            return usuario is null ? throw new UsuarioException("Usuario no valido") : usuario.Id;
        }
    }
}
