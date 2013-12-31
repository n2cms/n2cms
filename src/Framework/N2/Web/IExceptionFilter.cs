using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Web
{
    public interface IExceptionFilter
    {
        /// <summary>
        /// Returns <c>true</c> when an exception is interesting enough to send a message about.
        /// </summary>
        bool IsMessageworthy(Exception ex);
    }
}
