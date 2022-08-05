using OnePlace.Client.ComponentesGenericos.Base;
using OnePlace.Client.ComponentesGenericos.Utilities;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnePlace.Client.ComponentesGenericos.Switch
{
    public partial class Switch<T> : SimBooleanInput<T>
    {
        protected string Classname =>
        new CssBuilder("custom-control-label")            
          .AddClass(Class)
        .Build();
        protected string SwitchClassname =>
        new CssBuilder("sim-button-root sim-icon-button sim-switch-base")           
        .Build();       

        /// <summary>
        /// The text/label will be displayed next to the switch if set.
        /// </summary>
        [Parameter] public string Label { get; set; }

        /// <summary>
        /// If true, disables ripple effect.
        /// </summary>
        [Parameter] public bool DisableRipple { get; set; }
        [Parameter] public RenderFragment ChildContent { get; set; }

        //estos parametros se pusieron por que el css boostrap necesita esto para que funcionen los estilos del switch , en el original de mud blazor no los necesita!
        [Parameter] public string IdTag { get; set; }
        [Parameter] public string ForTag { get; set; }
    }
}
