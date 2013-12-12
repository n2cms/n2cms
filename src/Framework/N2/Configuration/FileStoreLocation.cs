using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Configuration
{
    /// <summary>
    /// Possible storage locations for uploaded files.
    /// </summary>
    public enum FileStoreLocation
    {
        /// <summary>Stored on the local file system.</summary>
        Disk,
        /// <summary>Stored in the database.</summary>
        Database
    }
}
