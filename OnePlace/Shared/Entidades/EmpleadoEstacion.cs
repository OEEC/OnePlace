using OnePlace.Shared.Entidades.SimsaCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnePlace.Shared.Entidades
{
    public class EmpleadoEstacion
    {
        public int EmpleadoId { get; set; }
        public int EstacionId { get; set; }

        [NotMapped]
        public Empleado Empleado { get; set; } = null!;
        [NotMapped]
        public Estacion Estacion { get; set; } = null!;
    }
}
