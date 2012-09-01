using System;
using System.Collections.Generic;
using N2.Engine;

namespace N2.Extensions.Tests.Fakes
{
	public class FakeTypeFinder2 : AppDomainTypeFinder
	{
		public Dictionary<Type, IList<Type>> typeMap = new Dictionary<Type, IList<Type>>();
		public override IEnumerable<Type> Find(Type requestedType)
		{
			if (typeMap.ContainsKey(requestedType))
				return typeMap[requestedType];
			return base.Find(requestedType);
		}
	}
}