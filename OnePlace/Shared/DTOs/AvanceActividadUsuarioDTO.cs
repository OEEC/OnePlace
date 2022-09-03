using OnePlace.Shared.Entidades;
using OnePlace.Shared.Entidades.SimsaCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnePlace.Shared.DTOs
{
    public class AvanceActividadUsuarioDTO
    {
        public List<ActividadUsuario> ListaActividades { get; set; }        
        public List<UsuarioDTO> ListaUsarios { get; set; }
    }
}
