﻿using OnePlace.Client.ComponentesGenericos.Base;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnePlace.Client.ComponentesGenericos.Elemento
{
    public class Elemento : SimComponentBase
    {
        /// <summary>
        /// Child content
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// The HTML element that will be rendered in the root by the component
        /// </summary>
        [Parameter] public string HtmlTag { get; set; }
        /// <summary>
        /// The ElementReference to bind to.
        /// Use like @bind-Ref="myRef"
        /// </summary>
        [Parameter] public ElementReference? Ref { get; set; }

        [Parameter] public EventCallback<ElementReference> RefChanged { get; set; }


        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            base.BuildRenderTree(builder);
            //Open
            builder.OpenElement(0, HtmlTag);

            //splatted attributes
            builder.AddMultipleAttributes(1, UserAttributes);
            //Class
            builder.AddAttribute(2, "class", Class);
            //Style
            builder.AddAttribute(3, "style", Style);

            // StopPropagation
            //the order matters. This has to be before content is added
            if (HtmlTag == "button")
                builder.AddEventStopPropagationAttribute(5, "onclick", true);

            //Reference capture
            if (Ref != null)
            {
                builder.AddElementReferenceCapture(6, async capturedRef =>
                {
                    Ref = capturedRef;
                    await RefChanged.InvokeAsync(Ref.Value);
                });
            }

            //Content
            builder.AddContent(10, ChildContent);

            //Close
            builder.CloseElement();
        }
    }
}
