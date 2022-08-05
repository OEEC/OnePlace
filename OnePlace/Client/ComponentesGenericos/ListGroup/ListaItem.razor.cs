using OnePlace.Client.ComponentesGenericos.Base;
using OnePlace.Client.ComponentesGenericos.Utilities;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OnePlace.Client.ComponentesGenericos.ListGroup
{
    public partial class ListaItem : SimComponentBase
    {
        protected string Classname =>
        new CssBuilder("sim-list-item")  
          .AddClass(Class)
        .Build();

        [Inject] protected NavigationManager UriHelper { get; set; }

        [CascadingParameter] protected Lista Lista { get; set; }

        /// <summary>
        /// The text to display
        /// </summary>
        [Parameter] public string Text { get; set; }

        /// <summary>
        /// Avatar to use if set.
        /// </summary>
        [Parameter] public string Avatar { get; set; }

        /// <summary>
        /// Link to a URL when clicked.
        /// </summary>
        [Parameter] public string Href { get; set; }

        /// <summary>
        /// Display content of this list item. If set, this overrides Text
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Add child list items here to create a nested list.
        /// </summary>
        [Parameter] public RenderFragment NestedList { get; set; }      

    }
}
