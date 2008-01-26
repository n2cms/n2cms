using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Diagnostics;

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
			Debug.WriteLine("InitializerModule: Init");
			Context.Initialize(false);
			Context.Instance.Attach(context);
		}

		public void Dispose()
		{
			Debug.WriteLine("InitializerModule: Dispose");
		}
	}
}
