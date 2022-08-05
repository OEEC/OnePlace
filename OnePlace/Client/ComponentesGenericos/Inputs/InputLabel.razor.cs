using OnePlace.Client.ComponentesGenericos.Base;
using OnePlace.Client.ComponentesGenericos.Utilities;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnePlace.Client.ComponentesGenericos.Inputs
{
    public partial class InputLabel : SimComponentBase
    {
        protected string Classname =>
       new CssBuilder()
         .AddClass("sim-input-label")        
         .AddClass(Class)
       .Build();

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// If true, the input element will be disabled.
        /// </summary>
        [Parameter] public bool Disabled { get; set; }

        /// <summary>
        /// If true, the label will be displayed in an error state.
        /// </summary>
        [Parameter] public bool Error { get; set; }
    }
}
