using System;
using N2.Configuration;
using N2.Engine;
using N2.Installation;
using N2.Web;

namespace N2.Tests.Fakes
{
	public class FakeRequestLifeCycleHandler : RequestLifeCycleHandler
	{
		public FakeRequestLifeCycleHandler(IWebContext webContext, InstallationManager installer, IRequestDispatcher dispatcher, IErrorHandler errors, EditSection editConfig, HostSection hostConfig)
			: base(webContext, EventBroker.Instance, installer, dispatcher, errors, editConfig)
		{
			initialized = true;
		}
		public FakeRequestLifeCycleHandler(IWebContext webContext, InstallationManager installer, IRequestDispatcher dispatcher, IErrorHandler errors)
			: base(webContext, EventBroker.Instance, installer, dispatcher, errors)
		{
			initialized = true;
		}

		public void BeginRequest()
		{
			base.Application_BeginRequest(this, new EventArgs());
		}
	}
}
