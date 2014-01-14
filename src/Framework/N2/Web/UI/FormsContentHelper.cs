using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine;

namespace N2.Web.UI
{
    public class FormsContentHelper : ContentHelperBase
    {
        public FormsContentHelper(Func<IEngine> engineGetter, Func<PathData> pathGetter)
            : base(engineGetter, pathGetter)
        {
        }
    }
}
