using System;
namespace N2.Web
{
    public interface IErrorHandler
    {
        void Notify(Exception ex);
    }
}
