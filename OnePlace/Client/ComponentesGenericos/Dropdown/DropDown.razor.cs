using OnePlace.Client.ComponentesGenericos.Base;
using OnePlace.Client.ComponentesGenericos.Utilities;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnePlace.Client.ComponentesGenericos.Dropdown
{
    public partial class DropDown<TItem> : SimComponentBase
    {
        protected string Classname =>
          new CssBuilder("dropdown-toggle")
         .AddClass(Class)
         .Build();

        //estas clases las puse para que pueda darle mas estilos a los dropdown, sino se quedarian todos con el mismo estilo y el template necesita variedad
        protected string DropdownClass => new CssBuilder("dropdown")
         .AddClass(ClassDropdown)
         .Build();
        protected string DropdownMenuClass => new CssBuilder("dropdown-menu")
          .AddClass(ClassDropdownMenu)
          .Build();
        [Parameter] public string ClassDropdown { get; set; }
        [Parameter] public string ClassDropdownMenu { get; set; }


        //originales
        [Parameter] public RenderFragment InitialTip { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        [Parameter] public EventCallback<TItem> OnSelected { get; set; }

        private bool show = false;
        private RenderFragment Tip;

        protected override void OnInitialized()
        {
            this.Tip = InitialTip;
        }

        public async Task HandleSelect(TItem item, RenderFragment<TItem> contentFragment)
        {
            this.Tip = contentFragment.Invoke(item);
            this.show = false;
            StateHasChanged();
            await this.OnSelected.InvokeAsync(item);
        }
    }
}
