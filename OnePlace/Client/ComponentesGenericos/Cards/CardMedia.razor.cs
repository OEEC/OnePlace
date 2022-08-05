using OnePlace.Client.ComponentesGenericos.Base;
using OnePlace.Client.ComponentesGenericos.Utilities;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnePlace.Client.ComponentesGenericos.Cards
{
    public partial class CardMedia : SimComponentBase
    {
        protected string StyleString =>
            StyleBuilder.Default($"background-image:url({_imageUrl});{_height};")
                .AddStyle(this.Style)
                .Build();

        protected string Classname =>
            new CssBuilder("pro-card-media")
                .AddClass(Class)
                .Build();

        [Parameter] public string Title { get; set; }

        [Parameter] public string Image { get; set; }

        [Parameter] public int Height { get; set; }

        private string _height;
        private string _imageUrl;

        protected override void OnInitialized()
        {
            _height = $"height: {Height}px";
            _imageUrl = "\"" + Image + "\"";
        }
    }
}
