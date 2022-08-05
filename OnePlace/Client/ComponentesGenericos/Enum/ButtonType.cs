using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace OnePlace.Client.ComponentesGenericos.Enum
{
    public enum ButtonType
    {
        [Description("button")]
        Button,
        [Description("submit")]
        Submit,
        [Description("reset")]
        Reset
    }
}
