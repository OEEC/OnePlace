using OnePlace.Shared.IdentityModels;
using System.Threading.Tasks;

namespace OnePlace.Server.Helpers
{
    public interface IUsuarioHelper
    {
        public Task<string> GetUsuarioId();
        public Task<IdentityUsuario> GetUsuario();
    }
}
