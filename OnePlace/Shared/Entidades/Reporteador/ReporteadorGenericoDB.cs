using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnePlace.Shared.Entidades.Reporteador
{
    public class ReporteGenericoDB
    {
        public Guid ReporteGenericoDBId { get; set; }
        public int Logo { get; set; }
        public virtual Elemento Titulo { get; set; }
        public virtual Elemento SubTitulo { get; set; }
        public DateTime FechaGeneracion { get; set; }
        public virtual ICollection<Columna> Columnas { get; set; } = new List<Columna>();
        public virtual ICollection<Renglon> Renglones { get; set; } = new List<Renglon>();
    }
    public class Subreporte
    {
        public virtual ICollection<Columna> Columnas { get; set; } = new List<Columna>();
        public virtual ICollection<Renglon> Renglones { get; set; } = new List<Renglon>();
    }
    public class Elemento
    {
        public Guid ElementoID { get; set; }
        public string Texto { get; set; }
    }
    public class ElementoR
    {
        public Guid ElementoRID { get; set; }
        public string Texto { get; set; }
        public string Columna { get; set; }
    }
    public class Columna
    {
        public Guid ColumnaID { get; set; }
        public string Texto { get; set; }
        public int ColSpan { get; set; }
        public int Alineacion { get; set; }
        public int Tamaño { get; set; }
    }
    public class Renglon
    {
        public Guid RenglonId { get; set; }
        public virtual ICollection<ElementoR> Elementos { get; set; }
    }
    public enum TipoReporte
    {
        Etiqueta30_20,
        ResponsivaEmpleado,
        ResponsivaEstacion,
        Politicas,
        Pagare,
        OrdendeServicio,
        Dictamen,
        TarjetadeCumple,
        Certificado
        //TicketVenta,
        //TicketNomina,
        //TransaccionAlmacen,       
        //Etiqueta30_30,
        //ReporteMensual
    }
}
