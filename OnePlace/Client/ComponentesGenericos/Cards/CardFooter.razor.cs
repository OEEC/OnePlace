using OnePlace.Client.ComponentesGenericos.Base;
using OnePlace.Client.ComponentesGenericos.Utilities;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnePlace.Client.ComponentesGenericos.Cards
{
    public partial class CardFooter : SimComponentBase
    {
        protected string Classname =>
        new CssBuilder("sim-card-actions")
          .AddClass(Class)
        .Build();

        /// <summary>
        /// Child content of the component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }
    }
}
