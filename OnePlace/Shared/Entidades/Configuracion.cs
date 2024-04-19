using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnePlace.Shared.Entidades
{
    public class Configuracion
    {
        public int Id { get; set; }
        [StringLength(20)]
        public string Tipo { get; set; } = string.Empty;
        [StringLength(250)]
        public string Valor { get; set; } = string.Empty;
        public bool Estatus { get; set; } = false;
    }
}
