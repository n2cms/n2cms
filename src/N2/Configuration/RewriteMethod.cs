using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Configuration
{
    /// <summary>
    /// Represents a way to map a friendly url to a template.
    /// </summary>
    public enum RewriteMethod
    {
        /// <summary>Use HttpContext.RewriteRequest.</summary>
        RewriteRequest,
        /// <summary>Use HttpServerUtility.TransferRequest.</summary>
        TransferRequest,
        /// <summary>Do not rewrite the request. Someone else should pick it up and do something about it.</summary>
        None
    }
}
