using N2.Engine;
using N2.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Management.Api
{
    public class ManagementModuleAttribute : ServiceAttribute
    {
        public ManagementModuleAttribute()
            : base(typeof(ManagementModuleBase))
        {
        }
    }

    public abstract class ManagementModuleBase : IAutoStart
    {
        public virtual IEnumerable<string> ScriptIncludes { get { yield break; } }
        public virtual IEnumerable<string> StyleIncludes { get { yield break; } }

        public virtual void Start()
        {
        }

        public virtual void Stop()
        {
        }
    }
}
