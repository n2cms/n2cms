using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using N2.Engine;
using N2.Definitions;

namespace N2.Persistence.Proxying
{
	public class AttributedProperty
	{
		public PropertyInfo Property { get; set; }
		public IInterceptableProperty Attribute { get; set; }
	}

	/// <summary>
	/// Creates a proxy that rewires auto-generated properties to detail get/set.
	/// </summary>
	[Service(typeof(IProxyFactory))]
	public class InterceptingProxyFactory : EmptyProxyFactory
	{
		class Tuple
		{
			public Type Type;
            public IInterceptorBuilder Builder;
			public Func<IInterceptableType, bool>[] UnproxiedSaveSetters;
		}

		private readonly Dictionary<string, Tuple> types = new Dictionary<string, Tuple>();
		private readonly ProxyGenerator generator = new ProxyGenerator();
		private readonly Type[] additionalInterfacesToProxy = new Type[] { 
            typeof(IInterceptedType)
            //, typeof(IInterceptableType)
		};

		public override void Initialize(IEnumerable<ItemDefinition> interceptedTypes)
		{
			foreach (var definition in interceptedTypes)
			{
				var type = definition.ItemType;
				var interceptableProperties = GetInterceptableProperties(definition).ToList();
				if (interceptableProperties.Count == 0)
					continue;

				var interceptor = new DetailPropertyInterceptor(type, interceptableProperties);
				types[type.FullName] = new Tuple 
				{ 
					Type = type, 
					Builder = interceptor,
					UnproxiedSaveSetters = GetSaveSetters(interceptableProperties).ToArray()
				};
			}
		}

		private IEnumerable<Func<IInterceptableType, bool>> GetSaveSetters(IEnumerable<AttributedProperty> interceptableProperties)
		{
			foreach (var property in interceptableProperties)
			{
				var pi = property.Property;
				Type propertyType = pi.PropertyType;
				string propertyName = pi.Name;
				MethodInfo getter = pi.GetGetMethod();
				MethodInfo setter = pi.GetSetMethod();

				if (property.Attribute.PersistAs == PropertyPersistenceLocation.DetailCollection)
				{
					if (!typeof(IEnumerable).IsAssignableFrom(propertyType))
						throw new InvalidOperationException("The property type of '" + propertyName + "' on '" + pi.DeclaringType + "' does not implement IEnumerable which is required for properties stored in a detail collection");

					yield return (interceptable) =>
						{
							IEnumerable propertyValue = (IEnumerable)getter.Invoke(interceptable, null);
							var collectionValues = interceptable.GetValues(propertyName);

							if (propertyValue == null && collectionValues == null)
								return false;

							interceptable.SetValues(propertyName, collectionValues);
							return true;
						};
				}
				else
				{
					yield return (interceptable) =>
						{
							object propertyValue = getter.Invoke(interceptable, null);
							object detailValue = interceptable.GetValue(propertyName);

							if (propertyValue == null && detailValue == null)
								return false;
							if (propertyValue != null && propertyValue.Equals(detailValue))
								return false;

							interceptable.SetValue(propertyName, propertyValue, propertyType);
							return true;
						};
				}
			}
		}

		private IEnumerable<AttributedProperty> GetInterceptableProperties(ItemDefinition definition)
		{
			// Also include properties on base classes since properties are matched by reference and
			// and we want to intercept calls to properties on base classes with the same name
			for (Type t = definition.ItemType; t != null; t = t.BaseType)
			{
				foreach (var property in t.GetProperties())
				{
					if (!property.CanRead)
						continue;
					if (!property.GetGetMethod().IsVirtual)
						continue;
					if (!property.IsCompilerGenerated())
						continue;

					var attributes = definition.GetCustomAttributes<IInterceptableProperty>(property.Name);
					if (attributes.Any(a => a.PersistAs != PropertyPersistenceLocation.Detail && a.PersistAs != PropertyPersistenceLocation.DetailCollection))
						// some property is persisted as something other than detail or detail collection
						continue;
					if (!attributes.Any(a => a.PersistAs == PropertyPersistenceLocation.Detail || a.PersistAs == PropertyPersistenceLocation.DetailCollection))
						// no property is persisted as detail or detail collection
						continue;

					yield return new AttributedProperty { Property = property, Attribute = attributes.First() };
				}
			}
		}

		public override object Create(string typeName, object id)
		{
			Tuple tuple;
			if (!types.TryGetValue(typeName, out tuple))
				return null;

            return generator.CreateClassProxy(tuple.Type, additionalInterfacesToProxy, tuple.Builder.Interceptor);
		}

		public override bool OnSaving(object instance)
		{
			return ApplyToDetailsOnUnproxiedInstance(instance);
		}

		private bool ApplyToDetailsOnUnproxiedInstance(object instance)
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
