using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.DynamicProxy;
using System.Reflection;

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
				var attribute = (IInterceptableProperty)property.GetCustomAttributes(typeof(IInterceptableProperty), true).FirstOrDefault();
				if (attribute == null || attribute.PersistAs != PropertyPersistenceMode.InterceptedDetails)
				    continue;

				var getMethod = property.GetGetMethod();
				if (getMethod == null)
					continue; // no public getter? let's move on
				if (!getMethod.IsCompilerGenerated())
					continue; // only intercept auto-implemented properties

				object defaultValue = GetDefaultValue(property.PropertyType, attribute.DefaultValue);

				methods[getMethod] = GetGetDetail(property.Name, defaultValue);

				var setMethod = property.GetSetMethod();
				if (setMethod == null)
					continue; // no public setter? that's okay
				methods[setMethod] = GetSetDetail(property.Name, defaultValue, property.PropertyType);
			}


			//for (var type = interceptedType; type != null; type = type.BaseType)
			//{
			//    foreach (var method in type.GetMethods())
			//    {
			//        if (!method.IsVirtual)
			//            continue;
			//        var action = PrepareMethod(method);
			//        if (action != null)
			//        {
			//            methods[method] = action;
			//            InterceptedProperties++;
			//        }
			//    }
			//}
		}

		public void Intercept(IInvocation invocation)
		{
			Action<IInvocation> action;
			if (methods.TryGetValue(invocation.Method, out action))
				action(invocation);
			else
				invocation.Proceed();
		}

		//private Action<IInvocation> PrepareMethod(PropertyInfo property, MethodInfo method, object defaultValue)
		//{
		//    string propertyName = property.Name;
			
		//    if (property.GetGetMethod() == method)
		//        return GetGetDetail(propertyName, defaultValue);

		//    if (property.GetSetMethod() == method)
		//        return GetSetDetail(propertyName, defaultValue, property.PropertyType);

		//    return null;
		//}

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

		private static Action<IInvocation> GetGetDetail(string propertyName, object defaultValue)
		{
			return invocation =>
			{
				var instance = invocation.Proxy as IInterceptableType;
				invocation.ReturnValue = instance.GetDetail(propertyName) ?? defaultValue;
			};
		}
	}
}
