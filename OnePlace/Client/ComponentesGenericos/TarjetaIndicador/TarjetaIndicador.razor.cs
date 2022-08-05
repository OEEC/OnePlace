using OnePlace.Client.ComponentesGenericos.Base;
using OnePlace.Client.ComponentesGenericos.Utilities;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnePlace.Client.ComponentesGenericos.TarjetaIndicador
{
    public partial class TarjetaIndicador : SimComponentBase
    {
        protected string Classname =>
        new CssBuilder("card-deck")
         .AddClass(Class)
      .Build();
        protected string StyleString =>
           StyleBuilder.Default($"background-image:url({_imageUrl})")
           .AddStyle(this.Style)
        .Build();
        [Parameter] public string Titulo { get; set; }
        [Parameter] public string Porcentaje { get; set; }
        [Parameter] public decimal CantidadInicial { get; set; }
        [Parameter] public decimal CantidadFinal { get; set; }
        [Parameter] public string ClaseBadge { get; set; }
        [Parameter] public string ColorTexto { get; set; }
        [Parameter] public string CounterName { get; set; }
        [Parameter] public string TextoLink { get; set; }
        [Parameter] public string Href { get; set; }
        [Parameter] public bool MostrarBadge { get; set; }
        [Parameter] public RenderFragment ChildContent { get; set; }
        [Parameter] public string Image { get; set; }
        private string _imageUrl;
        [Parameter] public EventCallback MetodoOnclick { get; set; }
        [Parameter] public bool UsarBotonEjecutable { get; set; }
        protected override void OnInitialized()
        {
            _imageUrl = "\"" + Image + "\"";
        }
        private Task BotonEjecutable()
        {
            return MetodoOnclick.InvokeAsync();
        }
    }
}
