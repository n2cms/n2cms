using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            public IInterceptorBuilder Builder;
			public Func<IInterceptableType, bool>[] UnproxiedSaveSetters;
		}

		private readonly Dictionary<string, Tuple> types = new Dictionary<string, Tuple>();
		private readonly ProxyGenerator generator = new ProxyGenerator();
		private readonly Type[] additionalInterfacesToProxy = new Type[] { 
            typeof(IInterceptedType)
            //, typeof(IInterceptableType)
		};

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
					Builder = interceptor,
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
					if (attributes.Any(a => a.PersistAs != PropertyPersistenceLocation.Detail && a.PersistAs != PropertyPersistenceLocation.DetailCollection))
						continue;
					if (!attributes.Any(a => a.PersistAs == PropertyPersistenceLocation.Detail || a.PersistAs == PropertyPersistenceLocation.DetailCollection))
						continue;

					yield return property;
				}
			}
		}

        class SpecificMethodsGenerationHook : IProxyGenerationHook
        {
            private HashSet<MethodInfo> interceptedMethods;
            object hashObject = new object(); 

            public SpecificMethodsGenerationHook(IEnumerable<MethodInfo> interceptedMethods)
            {
                this.interceptedMethods = new HashSet<MethodInfo>(interceptedMethods);
            }

            public void MethodsInspected()
            {
            }

            public void NonProxyableMemberNotification(Type type, MemberInfo memberInfo)
            {
            }

            public bool ShouldInterceptMethod(Type type, MethodInfo methodInfo)
            {
                return interceptedMethods.Contains(methodInfo);
            }

            #region Equals & GetHashCode

            public override bool Equals(object obj)
            {
                return obj.GetHashCode() == GetHashCode();
            }

            public override int GetHashCode()
            {
                return hashObject.GetHashCode();
            }

            #endregion
        }

		public override object Create(string typeName, object id)
		{
			Tuple tuple;
			if (!types.TryGetValue(typeName, out tuple))
				return null;

            var options = new ProxyGenerationOptions(new SpecificMethodsGenerationHook(tuple.Builder.GetInterceptedMethods()));
			return generator.CreateClassProxy(tuple.Type, additionalInterfacesToProxy, options, tuple.Builder.Interceptor);
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
