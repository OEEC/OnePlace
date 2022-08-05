using OnePlace.Client.ComponentesGenericos.Base;
using OnePlace.Client.ComponentesGenericos.Utilities;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnePlace.Client.ComponentesGenericos.TarjetaMenu
{
    public partial class TarjetaMenu : SimComponentBase
    {
        protected string Classname =>
         new CssBuilder("card-group")
        .AddClass(Class)
      .Build();
        [Parameter] public RenderFragment ChildContent { get; set; }
        [Parameter] public string Icon { get; set; }
        [Parameter] public string Titulo { get; set; }
        [Parameter] public string Subtitulo { get; set; }
        [Parameter] public string Href { get; set; }
        [Parameter] public string Color { get; set; }
        [Parameter] public string BackGroundColor { get; set; }
        [Parameter] public bool Acciones { get; set; }
        [Parameter] public string Editar { get; set; }
        [Parameter] public EventCallback Borrar { get; set; }
        [Parameter] public bool VerBorrarSoloActivo { get; set; }//se puso para que solo se vea el boton de eliminar si es activa la zona
        private Task BorrarOk()
        {
            return Borrar.InvokeAsync();
        }
    }
}
