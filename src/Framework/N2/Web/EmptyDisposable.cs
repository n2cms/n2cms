using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Web
{
    internal class EmptyDisposable : IDisposable
    {
        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion
    }
}
