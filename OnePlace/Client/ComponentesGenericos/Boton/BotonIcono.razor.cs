using OnePlace.Client.ComponentesGenericos.Base;
using OnePlace.Client.ComponentesGenericos.Utilities;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnePlace.Client.ComponentesGenericos.Boton
{
    public partial class BotonIcono : SimButtonBase
    {
        protected string Classname =>
        new CssBuilder("sim-button-root mud-icon-button")        
          .AddClass(Class)
        .Build();
      
        /// <summary>
        /// The Icon that will be used in the component.
        /// </summary>
        [Parameter] public string Icon { get; set; }

        /// <summary>
        /// Title of the icon used for accessibility.
        /// </summary>
        [Parameter] public string Title { get; set; }      

        /// <summary>
        /// Child content of component, only shows if Icon is null or Empty.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }     

    }
}
