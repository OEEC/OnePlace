using OnePlace.Client.ComponentesGenericos.Base;
using OnePlace.Client.ComponentesGenericos.Utilities;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnePlace.Client.ComponentesGenericos.Tabla
{
    public partial class Tabla<TItem> : SimComponentBase
    {
        //estas clases las puse para que pueda cambiar de tamaño y el estilo de la tabla 
        protected string TableClass => new CssBuilder("table table-bordered table-hover table-striped table-sm")
         .AddClass(ClassTable)
       .Build();
        protected string TheadClass => new CssBuilder("text-center")
          .AddClass(ClassThead)
        .Build();
        protected string TBodyClass => new CssBuilder("text-center")
        .AddClass(ClassTBody)
        .Build();
        [Parameter] public string ClassTable { get; set; }
        [Parameter] public string ClassThead { get; set; }
        [Parameter] public string ClassTBody { get; set; }

        [Parameter] public RenderFragment Cabecera { get; set; }
        [Parameter] public RenderFragment Pie { get; set; }
        [Parameter] public RenderFragment<TItem> Filas { get; set; }
        [Parameter] public IReadOnlyList<TItem> Items { get; set; }
    }
}
