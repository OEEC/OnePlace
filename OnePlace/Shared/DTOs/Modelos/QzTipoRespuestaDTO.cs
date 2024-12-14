namespace OnePlace.Shared.DTOs.Modelos
{
    public class QzTipoRespuestaDTO
    {
        public int IdTipoRespuesta { get; set; }
        public int? TipoPreguntaId { get; set; }
        public int? Cantidad { get; set; }
        public string Texto { get; set; } = string.Empty;
    }
}
