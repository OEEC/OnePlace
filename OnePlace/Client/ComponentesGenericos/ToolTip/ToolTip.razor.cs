using OnePlace.Client.ComponentesGenericos.Base;
using OnePlace.Client.ComponentesGenericos.Utilities;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace OnePlace.Client.ComponentesGenericos.ToolTip
{
    public partial class ToolTip : SimComponentBase
    {
        protected string ContainerClass => new CssBuilder("tooltip-wrapper")
            .Build();
        protected string Classname => new CssBuilder("toolspan")
            .AddClass(Class)
            .Build();

        /// <summary>
        /// Sets the text to be displayed inside the tooltip.
        /// </summary>
        [Parameter] public string Text { get; set; }

        /// <summary>
        /// Changes the default transition delay in milliseconds.
        /// </summary>
        [Parameter] public double Delay { get; set; } = 200;

        /// <summary>
        /// Changes the default transition delay in seconds.
        /// </summary>
        [Obsolete]
        [Parameter]
        public double Delayed
        {
            get { return Delay / 1000; }
            set { Delay = value * 1000; }
        }      

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Tooltip content. May contain any valid html
        /// </summary>
        [Parameter] public RenderFragment TooltipContent { get; set; }

        /// <summary>
        /// Determines if this component should be inline with it's surrounding (default) or if it should behave like a block element.
        /// </summary>
        [Parameter] public Boolean Inline { get; set; } = true;

        protected string GetTimeDelay()
        {
            return $"transition-delay: {Delay.ToString(CultureInfo.InvariantCulture)}ms;{Style}";
        }
    }
}
