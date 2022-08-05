using OnePlace.Client.ComponentesGenericos.Base;
using OnePlace.Client.ComponentesGenericos.Utilities;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnePlace.Client.ComponentesGenericos.Dropdown
{
    public partial class DropDownListLink : SimComponentBase
    {
        protected string Classname =>
        new CssBuilder("dropdown-item")
          .AddClass(Class)
        .Build();
        private Dictionary<string, object> Attributes
        {
            get => Disabled ? UserAttributes : new Dictionary<string, object>(UserAttributes)
            {
                { "href", Href },
                { "target", Target }
            };
        }
        [Parameter] public string Href { get; set; }
        [Parameter] public string Nombre { get; set; }
        [Parameter] public string Target { get; set; }
        [Parameter] public RenderFragment ChildContent { get; set; }
        [Parameter] public bool Disabled { get; set; }
    }
}
