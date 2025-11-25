using OnePlace.Shared.Enums;
using OnePlace.Shared.Filtros;
using System;

namespace OnePlace.Shared.DTOs
{
    public class QPreguntasFiltroDTO : Parametros_Busqueda_Gen
    {
        public int ZonaId { get; set; }
        public int EstacionId { get; set; }
        public int DepartamentoId { get; set; }
        public string Division { get; set; } = "TODAS";
        // Por defecto: últimos 7 días
        public DateTime Fecha_Inicio { get; set; } = DateTime.Today.AddDays(-7);
        public DateTime Fecha_Fin { get; set; } = DateTime.Today;
        public TipoQuiz TipoQuiz { get; set; }
        public TipoQuizEmpleado TipoQuizEmpleado { get; set; }
        public TipoUsuario TipoUsuario { get; set; } = TipoUsuario.TODOS;
        public bool Excel { get; set; } = false;
    }
}
