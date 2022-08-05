using OnePlace.Client.ComponentesGenericos.Base;
using OnePlace.Client.ComponentesGenericos.Utilities;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnePlace.Client.ComponentesGenericos.Avatar
{
    partial class Avatar : SimComponentBase
    {
        protected string Classname =>
        new CssBuilder("sim-avatar")         
          .AddClass(Class)
        .Build();

        protected string ClassAvatarImg =>
        new CssBuilder("rounded-circle")
         .AddClass(AvatarImgClass)
       .Build();

        [Parameter] public string AvatarImgClass { get; set; }

        /// <summary>
        /// If true, border-radius is set to 0.
        /// </summary>
        [Parameter] public bool Square { get; set; }

        /// <summary>
        /// If true, border-radius is set to the themes default value.
        /// </summary>
        [Parameter] public bool Rounded { get; set; }

        /// <summary>
        /// Link to image, if set a image will be displayed instead of text.
        /// </summary>
        [Parameter] public string Image { get; set; }      

        /// <summary>
        /// Child content of the component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }
    }
}
