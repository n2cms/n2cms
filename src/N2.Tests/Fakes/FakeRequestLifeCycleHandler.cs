using System;
using N2.Configuration;
using N2.Engine;
using N2.Installation;
using N2.Web;
using N2.Edit.Installation;

namespace N2.Tests.Fakes
{
	public class FakeRequestLifeCycleHandler : RequestLifeCycleHandler
	{
		public FakeRequestLifeCycleHandler(IWebContext webContext, InstallationManager installer, RequestPathProvider dispatcher, IContentAdapterProvider adapters, IErrorHandler errors, EditSection editConfig, HostSection hostConfig)
			: base(webContext, EventBroker.Instance, installer, dispatcher, adapters, errors, editConfig, hostConfig)
		{
			initialized = true;
		}

		public void BeginRequest()
		{
			base.Application_BeginRequest(this, new EventArgs());
		}
	}
}
