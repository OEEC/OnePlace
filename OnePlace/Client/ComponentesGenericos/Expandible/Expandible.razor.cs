using OnePlace.Client.ComponentesGenericos.Base;
using OnePlace.Client.ComponentesGenericos.Utilities;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnePlace.Client.ComponentesGenericos.Expandible
{
    public partial class Expandible : SimComponentBase
    {
        protected string Classname =>
        new CssBuilder("sim-collapse")
        .AddClass(Class)
        .Build();
        protected string Stylename =>
        new StyleBuilder()
        .AddStyle("color", "#2c7be5")
        .AddStyle(Style)
        .Build();

        [Parameter] public RenderFragment ChildContent { get; set; }
        [Parameter] public bool Collapsed { get; set; }
        [Parameter] public string Label { get; set; }
        void Toggle()
        {
            Collapsed = !Collapsed;
        }
    }
}
