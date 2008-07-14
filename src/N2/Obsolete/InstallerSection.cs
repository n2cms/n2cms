using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace N2.Configuration
{
    [Obsolete("This configuration section has been moved into a configuration element under the edit section: <installer.../> -> <edit><installer.../></edit>")]
    public class InstallerSection : ConfigurationSection
    {
    }
}
