namespace OnePlace.Shared.DTOs.Modelos
{
    public class EmpleadoDTO
    {
        public int Idempleado { get; set; }
        public string Noemp { get; set; }
        public string Division { get; set; }
        public EstacionDTO Estacion { get; set; }
        public PuestoDTO Puesto { get; set; }
        public PersonaDTO Persona { get; set; }
        public DepartamentoDTO Departamento { get; set; }
    }
}
