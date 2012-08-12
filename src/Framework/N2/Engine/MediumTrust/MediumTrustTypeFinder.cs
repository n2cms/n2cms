using System.Collections.Generic;
using System.Reflection;
using N2.Configuration;
using N2.Web;

namespace N2.Engine.MediumTrust
{
	public class MediumTrustTypeFinder : WebAppTypeFinder
	{
		private readonly EngineSection engineConfiguration;

		public MediumTrustTypeFinder(TypeCache assemblyCache, EngineSection engineConfiguration)
			: base(assemblyCache, engineConfiguration)
		{
			this.engineConfiguration = engineConfiguration;
		}

		public override IEnumerable<Assembly> GetAssemblies()
		{
			List<Assembly> assemblies = new List<Assembly>();
			foreach(var element in engineConfiguration.Assemblies.AllElements)
				assemblies.Add(Assembly.Load(element.Assembly));
			return assemblies;
		}
	}
}