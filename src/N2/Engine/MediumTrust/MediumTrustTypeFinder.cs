using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Configuration;
using N2.Engine;
using N2.Web;
using N2.Configuration;

namespace N2.Engine.MediumTrust
{
	public class MediumTrustTypeFinder : WebAppTypeFinder
	{
		private readonly EngineSection engineConfiguration;

		public MediumTrustTypeFinder(IWebContext webContext, EngineSection engineConfiguration)
			: base(webContext)
		{
			this.engineConfiguration = engineConfiguration;
		}

		public override IList<Assembly> GetAssemblies()
		{
			List<Assembly> assemblies = new List<Assembly>();
			foreach(var element in engineConfiguration.Assemblies.AllElements)
				assemblies.Add(Assembly.Load(element.Assembly));
			return assemblies;
		}
	}
}