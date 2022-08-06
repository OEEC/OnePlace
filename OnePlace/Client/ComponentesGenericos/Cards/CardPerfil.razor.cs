using OnePlace.Client.ComponentesGenericos.Base;
using OnePlace.Client.ComponentesGenericos.Utilities;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnePlace.Client.ComponentesGenericos.Cards
{
    public partial class CardPerfil : SimComponentBase
    {
        protected string Classname =>
        new CssBuilder("sim-typography")
        .AddClass(Class)
      .Build();
        [Parameter] public string Nombre { get; set; }
        [Parameter] public string UrlEditar { get; set; }
        [Parameter] public string UrlDetalles { get; set; }
        [Parameter] public int Id { get; set; }
        [Parameter] public string Imagen { get; set; }
        [Parameter] public bool VerBorrar { get; set; }
        [Parameter] public bool VerEditar { get; set; }
        [Parameter] public bool VerBorrarSoloActivo { get; set; }//se puso para que solo se vea el boton de eliminar si es activo el empleado
        [Parameter] public bool VerDetalles { get; set; }
        [Parameter] public EventCallback OnDeleted { get; set; }
    }
}
