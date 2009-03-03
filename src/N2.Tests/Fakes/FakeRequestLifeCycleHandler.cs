using System;
using N2.Configuration;
using N2.Installation;
using N2.Security;
using N2.Web;

namespace N2.Tests.Fakes
{
	public class FakeRequestLifeCycleHandler : RequestLifeCycleHandler
	{
		public FakeRequestLifeCycleHandler(ISecurityEnforcer security, IWebContext webContext, IErrorHandler errorHandler, InstallationManager installer, IRequestDispatcher dispatcher, EditSection editConfig, HostSection hostConfig)
			: base(webContext, installer, dispatcher, editConfig)
		{
			initialized = true;
		}
		public FakeRequestLifeCycleHandler(ISecurityEnforcer security, IWebContext webContext, IErrorHandler errorHandler, InstallationManager installer, IRequestDispatcher dispatcher)
			: base(webContext, installer, dispatcher)
		{
			initialized = true;
		}

		public void BeginRequest()
		{
			base.Application_BeginRequest(this, new EventArgs());
		}
	}
}
