using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Definitions;
using Castle.DynamicProxy;
using N2.Engine;

namespace N2.Persistence.Proxying
{
	/// <summary>
	/// Creates a proxy that rewires auto-generated properties to detail get/set.
	/// </summary>
	[Service(typeof(IProxyFactory))]
	public class InterceptingProxyFactory : EmptyProxyFactory
	{
		class Tuple
		{
			public Type Type;
			public IInterceptor Interceptor;
		}

		private readonly Dictionary<string, Tuple> types = new Dictionary<string, Tuple>();
		private readonly ProxyGenerator generator = new ProxyGenerator();
		private readonly Type[] additionalInterfacesToProxy = new Type[] { typeof(IInterceptedType), typeof(IInterceptableType) };

		public override void Initialize(IEnumerable<Type> interceptedTypes)
		{
			foreach (var type in interceptedTypes)
			{
				var interceptor = new DetailPropertyInterceptor(type);
				if (interceptor.InterceptedProperties == 0)
					continue;
				
				types[type.FullName] = new Tuple { Type = type, Interceptor = interceptor };
			}
		}

		public override object Create(string typeName)
		{
			Tuple tuple;
			if (!types.TryGetValue(typeName, out tuple))
				return null;

			return generator.CreateClassProxy(tuple.Type, additionalInterfacesToProxy, tuple.Interceptor);
		}
	}
}
