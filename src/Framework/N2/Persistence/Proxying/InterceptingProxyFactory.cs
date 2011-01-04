using System;
using System.Collections.Generic;
using System.Linq;
using Castle.DynamicProxy;
using N2.Engine;
using System.Reflection;

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
			public Func<IInterceptableType, bool>[] UnproxiedSaveSetters;
		}

		private readonly Dictionary<string, Tuple> types = new Dictionary<string, Tuple>();
		private readonly ProxyGenerator generator = new ProxyGenerator();
		private readonly Type[] additionalInterfacesToProxy = new Type[] { typeof(IInterceptedType), typeof(IInterceptableType) };

		public override void Initialize(IEnumerable<Type> interceptedTypes)
		{
			foreach (var type in interceptedTypes)
			{
				var interceptableProperties = GetInterceptableProperties(type).ToList();
				if (interceptableProperties.Count == 0)
					continue;

				var interceptor = new DetailPropertyInterceptor(type, interceptableProperties);
				types[type.FullName] = new Tuple 
				{ 
					Type = type, 
					Interceptor = interceptor,
					UnproxiedSaveSetters = GetSaveSetters(interceptableProperties).ToArray()
				};
			}
		}

		private IEnumerable<Func<IInterceptableType, bool>> GetSaveSetters(IEnumerable<PropertyInfo> interceptableProperties)
		{
			foreach (var pi in interceptableProperties)
			{
				Type propertyType = pi.PropertyType;
				string propertyName = pi.Name;
				MethodInfo getter = pi.GetGetMethod();
				MethodInfo setter = pi.GetSetMethod();

				yield return (interceptable) =>
					{
						object propertyValue = getter.Invoke(interceptable, null);
						object detailValue = interceptable.GetDetail(propertyName);

						if (propertyValue == null && detailValue == null)
							return false;
						if (propertyValue != null && propertyValue.Equals(detailValue))
							return false;
						
						interceptable.SetDetail(propertyName, propertyValue, propertyType);
						return true;
					};
			}
		}

		private IEnumerable<PropertyInfo> GetInterceptableProperties(Type type)
		{
			for (Type t = type; t != null; t = t.BaseType)
			{
				foreach (var property in t.GetProperties())
				{
					if (!property.CanRead)
						continue;
					if (!property.GetGetMethod().IsVirtual)
						continue;
					if (!property.IsCompilerGenerated())
						continue;

					var attributes = property.GetCustomAttributes(typeof(IInterceptableProperty), true).OfType<IInterceptableProperty>();
					if (attributes.Any(a => a.PersistAs != PropertyPersistenceMode.InterceptedDetails))
						continue;
					if (!attributes.Any(a => a.PersistAs == PropertyPersistenceMode.InterceptedDetails))
						continue;

					yield return property;
				}
			}
		}

		public override object Create(string typeName)
		{
			Tuple tuple;
			if (!types.TryGetValue(typeName, out tuple))
				return null;

			return generator.CreateClassProxy(tuple.Type, additionalInterfacesToProxy, tuple.Interceptor);
		}

		public override bool OnSaving(object instance)
		{
			if (instance is IInterceptedType)
				return false;

			Tuple tuple;
			if (!types.TryGetValue(instance.GetType().FullName, out tuple))
				return false;

			var interceptable = instance as IInterceptableType;
			bool altered = false;
			foreach (var setter in tuple.UnproxiedSaveSetters)
			{
				altered = setter(interceptable) || altered;
			}
			return altered;
		}
	}
}
