using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace N2.Configuration
{
    /// <summary>
    /// TODO: Refactor into N2.Extensions.dll
    /// </summary>
    public class FileListElement : ConfigurationElement
    {
        /// <summary>
        /// List of files to ignore, separate by spaces.
        /// </summary>
        [ConfigurationProperty("ignoreFiles", DefaultValue = "thumbs.db")]
        public string IgnoreFiles
        {
            get { return (string)base["ignoreFiles"]; }
            set { base["ignoreFiles"] = value; }
        }

        public IEnumerable<string> EnumerateIgnoreFiles()
        {
            return from x in IgnoreFiles.Split(',')
                   where (!String.IsNullOrWhiteSpace(x))
                   select x.Trim();
        }

    }
}
