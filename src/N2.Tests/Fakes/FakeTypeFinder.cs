using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine;
using System.Reflection;

namespace N2.Tests.Fakes
{
	public class FakeTypeFinder : ITypeFinder
	{
		readonly Assembly[] assemblies;
		readonly Type[] types;

		public FakeTypeFinder(Assembly assembly, params Type[] types)
		{
			assemblies = new[] {assembly};
			this.types = types;
		}
		public FakeTypeFinder(params Type[] types)
		{
			this.types = types;
		}
		public FakeTypeFinder(params Assembly[] assemblies)
		{
			this.assemblies = assemblies;
		}

		public IList<Type> Find(Type requestedType)
		{
			return types.Where(t => requestedType.IsAssignableFrom(requestedType)).ToList();
		}

		public IList<Assembly> GetAssemblies()
		{
			return assemblies.ToList();
		}
	}
}
