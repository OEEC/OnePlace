using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnePlace.Shared.Entidades
{
    public class ArchivoAdjunto
    {
        [Key]
        public int ArchivoAdjuntoId { get; set; }
        public string NombreArchivo { get; set; }
        public string ExtensionArchivo { get; set; }
        public byte[] ArchivoEnBytes { get; set; }
        public string UrlLocal { get; set; }
        public long Tamaño { get; set; }
        public DateTime? FechaDeSubida { get; set; }
        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"},
                {".xml", "application/xml"}
                //"application/octet-stream"
            };
        }
    }
}
