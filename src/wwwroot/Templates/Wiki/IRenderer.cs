using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

namespace N2.Templates.Wiki
{
    public interface IRenderer
    {
        Control AddTo(Control container, ViewContext context);
    }
}
