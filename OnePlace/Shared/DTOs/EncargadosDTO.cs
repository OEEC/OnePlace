using OnePlace.Shared.DTOs.Modelos;

namespace OnePlace.Shared.DTOs
{
    public class EncargadosDTO
    {
        public EmpleadoDTO Supervisor { get; set; } = null!;
        public EmpleadoDTO JefeTurno { get; set; } = null!;
        public EmpleadoDTO Gerente { get; set; } = null!;
    }
}
