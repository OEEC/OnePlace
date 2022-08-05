using OnePlace.Client.ComponentesGenericos.Base;
using OnePlace.Client.ComponentesGenericos.Utilities;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnePlace.Client.ComponentesGenericos.Cards
{
    public partial class CardHeader : SimComponentBase
    {
        protected string Classname =>
        new CssBuilder("sim-card-header")
          .AddClass(Class)
        .Build();

        /// <summary>
        /// If used renders child content of the CardHeaderAvatar.
        /// </summary>
        [Parameter] public RenderFragment CardCabeceraAvatar { get; set; }

        /// <summary>
        /// If used renders child content of the CardHeaderContent.
        /// </summary>
        [Parameter] public RenderFragment CardCabeceraContenido { get; set; }

        /// <summary>
        /// If used renders child content of the CardHeaderActions.
        /// </summary>
        [Parameter] public RenderFragment CardCabeceraAcciones { get; set; }

        /// <summary>
        /// Optional child content
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }
    }
}
