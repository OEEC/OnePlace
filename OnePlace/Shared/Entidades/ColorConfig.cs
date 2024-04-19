using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnePlace.Shared.Entidades
{
    public class ColorConfig
    {
        [Key]
        public int IdColorConfig { get; set; }
        public string Color { get; set; } = string.Empty;
        public bool Seleccionado { get; set; } = false;
    }
}
