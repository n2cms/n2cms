using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Configuration
{
    /// <summary>
    /// Base class used by N2's configuration sections.
    /// </summary>
    public abstract class ContentConfigurationSectionBase : ConfigurationSectionBase
    {
        /// <summary>Gets componetn configuration keys used to enable additional service registrations.</summary>
        /// <returns></returns>
        public virtual void ApplyComponentConfigurationKeys(List<string> configurationKeys)
        {
        }
    }
}
