using OnePlace.Client.ComponentesGenericos.Base;
using OnePlace.Client.ComponentesGenericos.Utilities;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnePlace.Client.ComponentesGenericos.TarjetaMenu
{
    public partial class TarjetaMenuTempleate : SimComponentBase
    {
        protected string Classname =>
         new CssBuilder("card-group")
        .AddClass(Class)
        .Build();
        [Parameter] public RenderFragment ChildContent { get; set; }
    }
}
