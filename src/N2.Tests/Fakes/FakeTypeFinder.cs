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
		public Assembly[] Assemblies { get; set; }
		public Type[] Types { get; set; }

		public FakeTypeFinder(Assembly assembly, params Type[] types)
		{
			Assemblies = new[] {assembly};
			this.Types = types;
		}
		public FakeTypeFinder(params Type[] types)
		{
			Assemblies = new Assembly[0];
			this.Types = types;
		}
		public FakeTypeFinder(params Assembly[] assemblies)
		{
			this.Assemblies = assemblies;
		}

		public IList<Type> Find(Type requestedType)
		{
			return Types.Where(t => requestedType.IsAssignableFrom(requestedType)).ToList();
		}

		public IList<Assembly> GetAssemblies()
		{
			return Assemblies.ToList();
		}
	}
}
