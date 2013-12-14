using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Web
{

    /// <summary>
    /// Alignment of a block of content. This could be a HTML DIV or other block element.
    /// </summary>
    public enum BlockAlignment : int
    {
        Left = 0x10,
        Right = 0x01,
        Middle = 0x11,
        None = 0x00
    }


}
