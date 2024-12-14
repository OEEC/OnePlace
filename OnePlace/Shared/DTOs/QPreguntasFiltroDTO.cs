using OnePlace.Shared.Enums;
using OnePlace.Shared.Filtros;

namespace OnePlace.Shared.DTOs
{
    public class QPreguntasFiltroDTO : Parametros_Busqueda_Gen
    {
        public int ZonaId { get; set; }
        public int EstacionId { get; set; }
        public int DepartamentoId { get; set; }
        public TipoQuiz TipoQuiz { get; set; }
    }
}
