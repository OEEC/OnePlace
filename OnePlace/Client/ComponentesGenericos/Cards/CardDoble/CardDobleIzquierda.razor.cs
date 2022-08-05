using OnePlace.Client.ComponentesGenericos.Base;
using OnePlace.Client.ComponentesGenericos.Utilities;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnePlace.Client.ComponentesGenericos.Cards.CardDoble
{
    public partial class CardDobleIzquierda : SimComponentBase
    {
        protected string Classname =>
        new CssBuilder("col-lg-8 pr-lg-2 mb-3")
          .AddClass(Class)
        .Build();
        [Parameter] public RenderFragment ChildContent { get; set; }
    }
}
