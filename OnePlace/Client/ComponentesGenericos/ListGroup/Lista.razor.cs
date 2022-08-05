using OnePlace.Client.ComponentesGenericos.Base;
using OnePlace.Client.ComponentesGenericos.Utilities;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnePlace.Client.ComponentesGenericos.ListGroup
{
    public partial class Lista : SimComponentBase
    {
        protected string Classname =>
        new CssBuilder("sim-list")           
          .AddClass(Class)
        .Build();

        [CascadingParameter] protected Lista ParentList { get; set; }

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Set true to make the list items clickable. This is also the precondition for list selection to work.
        /// </summary>
        [Parameter] public bool Clickable { get; set; }     
       
        /// <summary>
        /// If true, will disable the list item if it has onclick.
        /// </summary>
        [Parameter] public bool Disabled { get; set; }       
    }
}
