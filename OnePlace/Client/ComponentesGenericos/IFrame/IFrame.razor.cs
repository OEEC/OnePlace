using OnePlace.Client.ComponentesGenericos.Base;
using OnePlace.Client.ComponentesGenericos.Utilities;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnePlace.Client.ComponentesGenericos.IFrame
{
    partial class IFrame : SimComponentBase
    {
        protected string Classname =>
        new CssBuilder("frame")
          .AddClass(Class)
        .Build();
        [Parameter] public string Src { get; set; }
        [Parameter] public string Id { get; set; }        
    }
}
