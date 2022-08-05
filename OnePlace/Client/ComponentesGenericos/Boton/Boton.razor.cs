using OnePlace.Client.ComponentesGenericos.Base;
using OnePlace.Client.ComponentesGenericos.Utilities;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnePlace.Client.ComponentesGenericos.Boton
{
    public partial class Boton : SimButtonBase
    {
        protected string Classname =>
        new CssBuilder("sim-button-root mud-button")         
          .AddClass(Class)
        .Build();

        protected string StartIconClass =>
        new CssBuilder("sim-button-icon-start")         
          .AddClass(IconClass)
        .Build();

        protected string EndIconClass =>
        new CssBuilder("sim-button-icon-end")          
          .AddClass(IconClass)
        .Build();

        /// <summary>
        /// Icon placed before the text if set.
        /// </summary>
        [Parameter] public string StartIcon { get; set; }

        /// <summary>
        /// Icon placed after the text if set.
        /// </summary>
        [Parameter] public string EndIcon { get; set; }       

        /// <summary>
        /// Icon class names, separated by space
        /// </summary>
        [Parameter] public string IconClass { get; set; }     

        /// <summary>
        /// If true, the button will take up 100% of available width.
        /// </summary>
        [Parameter] public bool FullWidth { get; set; }

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }
    }
}
