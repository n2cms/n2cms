using System;
using N2.Configuration;
using N2.Engine;
using N2.Web;

namespace N2.Tests.Fakes
{
    public class FakeRequestLifeCycleHandler : RequestLifeCycleHandler
    {
        public FakeRequestLifeCycleHandler(IWebContext webContext, RequestPathProvider dispatcher, IContentAdapterProvider adapters, IErrorNotifier errors, ConfigurationManagerWrapper configuration)
            : base(webContext, EventBroker.Instance, dispatcher, adapters, errors, configuration)
        {
            initialized = true;
        }

        public void BeginRequest()
        {
            base.Application_BeginRequest(this, new EventArgs());
        }

        public void PostMapRequestHandler()
        {
            base.Application_PostMapRequestHandler(this, new EventArgs());
        }

        public void PostResolveRequestCache()
        {
            base.Application_PostResolveRequestCache(this, new EventArgs());
        }
    }
}
