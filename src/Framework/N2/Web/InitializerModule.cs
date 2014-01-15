using System.Web;
using N2.Engine;

namespace N2.Web
{
    /// <summary>
    /// A HttpModule that ensures that the N2 engine is initialized with a web 
    /// context.
    /// </summary>
    public class InitializerModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            EventBroker.Instance.Attach(context);
            N2.Context.Initialize(false);
        }

        public void Dispose()
        {
        }
    }
}
