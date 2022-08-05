using OnePlace.Client.ComponentesGenericos.Base;
using OnePlace.Client.ComponentesGenericos.Utilities;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnePlace.Client.ComponentesGenericos.CasillaVerificacion
{
    public partial class CasillaVerificacion<T> : SimBooleanInput<T>
    {
        protected string Classname =>
        new CssBuilder("sim-checkbox")            
          .AddClass(Class)
        .Build();

        protected string CheckBoxClassname =>
        new CssBuilder("sim-button-root mud-icon-button")            
            .AddClass($"sim-disabled", Disabled)
        .Build();
        protected string InputClass =>
        new CssBuilder("sim-button-root mud-icon-button")
          .AddClass(ClassInput)
       .Build();

        [Parameter] public string ClassInput { get; set; }

        /// <summary>
        /// If applied the text will be added to the component.
        /// </summary>
        [Parameter] public string Label { get; set; }

        /// <summary>
        /// If true, disables ripple effect.
        /// </summary>
        [Parameter] public bool DisableRipple { get; set; }

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        [Parameter] public bool TriState { get; set; }

        //controlar estado se activo o inactivo con una casilla de verificacion
        [Parameter] public bool ManejarEstados { get; set; }
        protected override Task OnChange(ChangeEventArgs args)
        {
            Touched = true;

            // Apply only when TriState parameter is set to true and T is bool?
            if (TriState && typeof(T) == typeof(bool?))
            {
                // The cycle is forced with the following steps: true, false, indeterminate, true, false, indeterminate...
                if (!((bool?)(object)_value).HasValue)
                {
                    return SetBoolValueAsync(true);
                }
                else
                {
                    return ((bool?)(object)_value).Value ? SetBoolValueAsync(false) : SetBoolValueAsync(default);
                }
            }
            else
            {
                return SetBoolValueAsync((bool?)args.Value);
            }
        }
    }
}
