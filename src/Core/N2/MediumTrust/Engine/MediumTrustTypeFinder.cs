using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Configuration;
using N2.MediumTrust.Configuration;
using N2.Engine;
using N2.Web;

namespace N2.MediumTrust.Engine
{
	public class MediumTrustTypeFinder : WebAppTypeFinder
	{
		private readonly MediumTrustSectionHandler configSection;

		public MediumTrustTypeFinder(IWebContext webContext)
			: base(webContext)
		{
			configSection = (MediumTrustSectionHandler)WebConfigurationManager.GetSection("n2/mediumTrust");
		}

		//public IList<Type> Find(Type requestedType)
		//{
		//    List<Type> types = new List<Type>();
		//    foreach (Assembly a in GetAssemblies())
		//    {
		//        foreach(
		//    }
		//    //if (requestedType == typeof(ContentItem))
		//    //{
		//    //    List<Type> types = new List<Type>();
		//    //    foreach (TypeElement element in configSection.ItemTypes)
		//    //        types.Add(Type.GetType(element.TypeName));
		//    //    return types;
		//    //}
		//    //else
		//    //    throw new NotSupportedException("The MediumTrustTypeFinder can only find item types.");
		//}

		public override IList<Assembly> GetAssemblies()
		{
			List<Assembly> assemblies = new List<Assembly>();
			foreach(AssemblyInfo element in configSection.Assemblies)
				assemblies.Add(Assembly.Load(element.Assembly));
			return assemblies;
		}
	}
}