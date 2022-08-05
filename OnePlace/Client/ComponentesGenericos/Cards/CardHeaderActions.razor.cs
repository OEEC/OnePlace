using OnePlace.Client.ComponentesGenericos.Base;
using OnePlace.Client.ComponentesGenericos.Utilities;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnePlace.Client.ComponentesGenericos.Cards
{
    public partial class CardHeaderActions : SimComponentBase
    {
        protected string Classname =>
        new CssBuilder("card-header")
          .AddClass(Class)
        .Build();
        [Parameter] public string Titulo { get; set; }
        [Parameter] public string Url { get; set; }
        [Parameter] public string TituloBtn { get; set; }
        [Parameter] public RenderFragment AreadeBotones { get; set; }
        [Parameter] public AccionesTabla AccionesTablaBtn { get; set; }
        public enum AccionesTabla
        {
            Nuevo,
            Filtrar,
            Exportar,
            Custom
        }
    }
}
