using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace OnePlace.Client.ComponentesGenericos.Enum
{
    public enum DividerType
    {
        [Description("fullwidth")]
        FullWidth,
        [Description("inset")]
        Inset,
        [Description("middle")]
        Middle
    }
}
