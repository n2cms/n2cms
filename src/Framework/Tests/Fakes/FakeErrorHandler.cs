using System;
using N2.Web;
using NUnit.Framework;

namespace N2.Tests.Fakes
{
    public class FakeErrorHandler : IErrorNotifier
    {
        #region IErrorNotifier Members

        public void Notify(Exception ex)
        {
            Assert.Fail(ex.ToString());
            ErrorOccured(this, new ErrorEventArgs { Error = ex });
        }

        public event EventHandler<ErrorEventArgs> ErrorOccured = delegate { };

        #endregion
    }
}
