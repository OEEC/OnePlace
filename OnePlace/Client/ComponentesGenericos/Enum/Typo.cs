using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace OnePlace.Client.ComponentesGenericos.Enum
{
    public enum Typo
    {
        [Description("inherit")]
        inherit,
        [Description("h1")]
        h1,
        [Description("h2")]
        h2,
        [Description("h3")]
        h3,
        [Description("h4")]
        h4,
        [Description("h5")]
        h5,
        [Description("h6")]
        h6,
        [Description("subtitle1")]
        subtitle1,
        [Description("subtitle2")]
        subtitle2,
        [Description("body1")]
        body1,
        [Description("body2")]
        body2,
        [Description("button")]
        button,
        [Description("caption")]
        caption,
        [Description("overline")]
        overline,
        
        [Description("normal")]
        normal,
        [Description("strong")]
        strong,
        [Description("small")]
        small,
        [Description("del")]
        del,
    }
}
