using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnePlace.Shared.Entidades.SimsaCore
{
    public class AreaDepartamentoEmpresa
    {
        public int Iddce { get; set; }
        public int Idarea { get; set; }
        public int Iddepartamento { get; set; }
        public int Idempresa { get; set; }

        //[NotMapped] public Departamento Departamento { get; set; } = null!;
        //[NotMapped] public Empresa Empresa { get; set; } = null!;
        [NotMapped] public Empresa Empresa { get; set; }
        [NotMapped] public Departamento Departamento { get; set; }
    }
}
