using System;
using System.Collections.Generic;
using System.Text;
using N2.Engine;
using N2.Plugin;

namespace N2.Templates.Services
{
	[AutoInitialize]
	public class NotFoundInitializer : IPluginInitializer
	{
		public void Initialize(IEngine engine)
		{
			engine.AddComponent("notFoundHandler", typeof(NotFoundHandler));
		}
	}
}
