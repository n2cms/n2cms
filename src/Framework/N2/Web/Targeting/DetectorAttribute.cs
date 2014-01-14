using N2.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Web.Targeting
{
    public class DetectorAttribute : ServiceAttribute
    {
        public DetectorAttribute()
            : base(typeof(DetectorBase))
        {
        }
    }
}
