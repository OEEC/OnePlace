using OnePlace.Client.ComponentesGenericos.Base;
using OnePlace.Client.ComponentesGenericos.Enum;
using OnePlace.Client.ComponentesGenericos.Utilities;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnePlace.Client.ComponentesGenericos.Link
{
    public partial class Enlace : SimComponentBase
    {
        protected string Classname =>
        new CssBuilder("sim-typography sim-link")           
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

        /// <summary>
        /// Typography variant to use.
        /// </summary>
        [Parameter] public Typo Typo { get; set; } = Typo.body1;       

        /// <summary>
        /// The URL, which is the actual link.
        /// </summary>
        [Parameter] public string Href { get; set; }

        /// <summary>
        /// The target attribute specifies where to open the link, if Link is specified. Possible values: _blank | _self | _parent | _top | <i>framename</i>
        /// </summary>
        [Parameter] public string Target { get; set; }

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// If true, the navlink will be disabled.
        /// </summary>
        [Parameter] public bool Disabled { get; set; }
    }
}
