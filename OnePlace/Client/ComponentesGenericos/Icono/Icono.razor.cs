using OnePlace.Client.ComponentesGenericos.Base;
using OnePlace.Client.ComponentesGenericos.Utilities;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnePlace.Client.ComponentesGenericos.Icono
{
    public partial class Icono : SimComponentBase
    {
        protected string Classname =>
             new CssBuilder("sim-icon-root")
              .AddClass(Class)
              .Build();

        /// <summary>
        /// Icon to be used can either be svg paths for font icons.
        /// </summary>
        [Parameter] public string Icon { get; set; }

        /// <summary>
        /// Title of the icon used for accessibility.
        /// </summary>
        [Parameter] public string Title { get; set; }       

        /// <summary>
        /// The viewbox size of an svg element.
        /// </summary>
        [Parameter] public string ViewBox { get; set; } = "0 0 24 24";

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }
    }
}
