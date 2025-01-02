namespace OnePlace.Shared.DTOs.Modelos
{
    public class UsuarioAspDTO
    {
        public string Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string ApellidoPaterno { get; set; } = string.Empty;
        public string ApellidoMaterno { get; set; } = string.Empty;
        public string Noemp { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public EmpleadoDTO Empleado { get; set; } = null!;
    }
}
