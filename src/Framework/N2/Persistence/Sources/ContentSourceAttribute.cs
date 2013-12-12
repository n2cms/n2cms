using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine;

namespace N2.Persistence.Sources
{
    public class ContentSourceAttribute : ServiceAttribute
    {
        public ContentSourceAttribute()
            : base(typeof(SourceBase))
        {
        }
    }
}
