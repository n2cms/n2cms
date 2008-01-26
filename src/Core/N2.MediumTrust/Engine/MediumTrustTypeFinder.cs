using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Configuration;
using N2.MediumTrust.Configuration;

namespace N2.MediumTrust.Engine
{
	public class MediumTrustTypeFinder : N2.Engine.ITypeFinder
	{
		private readonly MediumTrustSectionHandler configSection;

		public MediumTrustTypeFinder()
		{
			configSection = (MediumTrustSectionHandler)WebConfigurationManager.GetSection("n2/mediumTrust");
		}

		public IList<Type> Find(Type requestedType)
		{
			if (requestedType == typeof(ContentItem))
			{
				List<Type> types = new List<Type>();
				foreach (TypeElement element in configSection.ItemTypes)
					types.Add(Type.GetType(element.TypeName));
				return types;
			}
			else
				throw new NotSupportedException("The MediumTrustTypeFinder can only find item types.");
		}

		public IList<Assembly> GetAssemblies()
		{
			List<Assembly> assemblies = new List<Assembly>();
			foreach(AssemblyInfo element in configSection.Assemblies)
				assemblies.Add(Assembly.Load(element.Assembly));
			return assemblies;
		}
	}
}