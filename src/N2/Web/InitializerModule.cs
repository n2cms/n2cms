using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Diagnostics;
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
			IEngine engine = Context.Initialize(false);
			engine.Attach(context);
		}

		public void Dispose()
		{
		}
	}
}
