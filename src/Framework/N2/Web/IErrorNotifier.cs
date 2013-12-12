using System;

namespace N2.Web
{
    public interface IErrorNotifier
    {
        void Notify(Exception ex);
        event EventHandler<ErrorEventArgs> ErrorOccured;
    }

}
