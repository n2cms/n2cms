using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using System.Collections;

namespace N2.Persistence.Proxying
{
	/// <summary>
	/// Intercepts detail property calls and calls Get/SetDetail.
	/// </summary>
	public class DetailPropertyInterceptor : IInterceptor
	{
		private readonly IDictionary<MethodInfo, Action<IInvocation>> methods = new Dictionary<MethodInfo, Action<IInvocation>>();
		private static readonly Action<IInvocation> proceedAction = (invocation) => invocation.Proceed();
		private static MethodInfo getEntityNameMethod = typeof(IInterceptedType).GetMethod("GetTypeName");
		
		public DetailPropertyInterceptor(Type interceptedType, IEnumerable<PropertyInfo> interceptedProperties)
		{
			string typeName = interceptedType.FullName;
			methods[getEntityNameMethod] = invocation => invocation.ReturnValue = typeName;
			
			Action<IInvocation> getContentTypeAction = invocation => invocation.ReturnValue = interceptedType;
			for (Type t = interceptedType; t != null; t = t.BaseType)
			{
				MethodInfo method = t.GetMethod("GetContentType");
				if (method != null)
					methods[method] = getContentTypeAction;
			}

			foreach (var property in interceptedProperties)
			{
				var attributes = property.GetCustomAttributes(typeof(IInterceptableProperty), true).OfType<IInterceptableProperty>();
				var attribute = attributes.FirstOrDefault();
				if (attribute == null || attributes.Any(a => a.PersistAs != PropertyPersistenceLocation.Detail && a.PersistAs != PropertyPersistenceLocation.DetailCollection))
				    continue;

				var location = attributes.Where(a => a.PersistAs == PropertyPersistenceLocation.Detail || a.PersistAs == PropertyPersistenceLocation.DetailCollection).Select(a => a.PersistAs).FirstOrDefault();

				var getMethod = property.GetGetMethod();
				if (getMethod == null)
					continue; // no public getter? let's move on
				if (!getMethod.IsCompilerGenerated())
					continue; // only intercept auto-implemented properties

				object defaultValue = GetDefaultValue(property.PropertyType, attribute.DefaultValue);

				var setMethod = property.GetSetMethod();
				if (location == PropertyPersistenceLocation.Detail)
				{
					methods[getMethod] = GetGetDetail(property.Name, defaultValue);

					if (setMethod == null)
						continue; // no public setter? that's okay
					methods[setMethod] = GetSetDetail(property.Name, defaultValue, property.PropertyType);
				}
				else if (location == PropertyPersistenceLocation.DetailCollection)
				{
					if (!typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
						throw new InvalidCastException("The property " + property.Name + " has the property type " + property.PropertyType + " but a type assignable from IEnumerable is required for usage of PropertyPersistenceLocation.DetailCollection");
					if (defaultValue != null && !typeof(IEnumerable).IsAssignableFrom(defaultValue.GetType()))
						throw new InvalidCastException("The property " + property.Name + " has a default value type " + property.PropertyType + " but a type assignable from IEnumerable is required for usage of PropertyPersistenceLocation.DetailCollection");

					methods[getMethod] = GetGetDetailCollection(property.Name, defaultValue);

					if (setMethod == null)
						continue; // no public setter? that's okay
					methods[setMethod] = GetSetDetailCollection(property.Name, defaultValue as IEnumerable, property.PropertyType);
				}
			}
		}

		public void Intercept(IInvocation invocation)
		{
			Action<IInvocation> action;
			if (methods.TryGetValue(invocation.Method, out action))
				action(invocation);
			else
				invocation.Proceed();
		}

		private static object GetDefaultValue(Type propertyType, object defaultValue)
		{
			if (defaultValue == null)
			{
				if (propertyType.IsValueType)
					defaultValue = Activator.CreateInstance(propertyType);
				else if (propertyType == typeof(string))
					defaultValue = string.Empty;
			}
			return defaultValue;
		}

		private static Action<IInvocation> GetGetDetail(string propertyName, object defaultValue)
		{
			return invocation =>
			{
				var instance = invocation.Proxy as IInterceptableType;
				invocation.ReturnValue = instance.GetDetail(propertyName) ?? defaultValue;
			};
		}

		private static Action<IInvocation> GetSetDetail(string propertyName, object defaultValue, Type valueType)
		{
			if (defaultValue != null)
				return invocation =>
				{
					var instance = invocation.Proxy as IInterceptableType;
					var value = invocation.Arguments[0];
					if (defaultValue.Equals(value))
						value = null;

					instance.SetDetail(propertyName, value, valueType);
				};
			else
				return invocation =>
				{
					var instance = invocation.Proxy as IInterceptableType;
					var value = invocation.Arguments[0];

					instance.SetDetail(propertyName, value, valueType);
				};
		}

		private Action<IInvocation> GetGetDetailCollection(string propertyName, object defaultValue)
		{
			return invocation =>
			{
				var instance = invocation.Proxy as IInterceptableType;
				invocation.ReturnValue = instance.GetDetailCollection(propertyName) ?? defaultValue;
			};
		}

		private Action<IInvocation> GetSetDetailCollection(string propertyName, IEnumerable defaultValue, Type type)
		{
			if(defaultValue != null)
				return invocation =>
				{
					var instance = invocation.Proxy as IInterceptableType;
					var value = invocation.Arguments[0] as IEnumerable;
					if (value == null)
						instance.SetDetailCollection(propertyName, value);
					else if(CollectionEquals(defaultValue, value))
						instance.SetDetailCollection(propertyName, null);
					else
						instance.SetDetailCollection(propertyName, value);
				};
			else
				return invocation =>
				{
					var instance = invocation.Proxy as IInterceptableType;
					var value = invocation.Arguments[0];

					instance.SetDetailCollection(propertyName, value as IEnumerable);
				};
		}

		private static bool CollectionEquals(IEnumerable defaultValue, IEnumerable value)
		{
			var dve = defaultValue.GetEnumerator();
			var ve = value.GetEnumerator();
			while (true)
			{
				var dveMoved = dve.MoveNext();
				var veMoved = ve.MoveNext();
				if (dveMoved && veMoved)
				{
					if (dve.Current == ve.Current)
						// elements are equal, advance
						continue;
					
					// elements not equal, collections differ
					return false;
				}
				else if (!dveMoved && !veMoved)
					// collection ended, collections are equal
					return true;

				// one collection ended but not the other, collections differ
				return false;
			}
		}
	}
}
