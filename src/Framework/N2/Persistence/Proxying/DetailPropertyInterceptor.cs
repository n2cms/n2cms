using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;

namespace N2.Persistence.Proxying
{
	//public class DetailPropertyLazyInitializer : AbstractLazyInitializer
	//{
	//    Type persistentClass;

	//    public DetailPropertyLazyInitializer(Type persistentClass, string entityName, object id, ISessionImplementor session)
	//        : base(entityName, id, session)
	//    {
	//        this.persistentClass = persistentClass;
	//    }
		
	//    public override Type PersistentClass
	//    {
	//        get { return persistentClass; }
	//    }

	//    public override void Initialize()
	//    {
	//        //base.Initialize();
	//    }
	//}

	//public class NHibernateProxyInterceptor : IInterceptor
	//{
	//    private static MethodInfo getNHibernateProxyMethod = typeof(INHibernateProxy).GetProperty("HibernateLazyInitializer").GetGetMethod();

	//    ILazyInitializer lazyItializer;

	//    public NHibernateProxyInterceptor(ILazyInitializer lazyItializer)
	//    {
	//        this.lazyItializer = lazyItializer;
	//    }

	//    #region IInterceptor Members

	//    public void Intercept(IInvocation invocation)
	//    {
	//        if (invocation.Method == getNHibernateProxyMethod)
	//            invocation.ReturnValue = lazyItializer;
	//        else invocation.Proceed();
	//    }

	//    #endregion
	//}

	/// <summary>
	/// Intercepts detail property calls and calls Get/SetDetail.
	/// </summary>
	public class DetailPropertyInterceptor : IInterceptor
	{
		private readonly IDictionary<MethodInfo, Action<IInvocation>> methods = new Dictionary<MethodInfo, Action<IInvocation>>();
		private static readonly Action<IInvocation> proceedAction = (invocation) => invocation.Proceed();
		private static MethodInfo getEntityNameMethod = typeof(IInterceptedType).GetMethod("GetTypeName");
		private static MethodInfo getPersistentClassMethod = typeof(NHibernate.Search.Util.IProxiedEntity).GetMethod("GetPersistentClass");
		
		public DetailPropertyInterceptor(Type interceptedType, IEnumerable<PropertyInfo> interceptedProperties)
		{
			string typeName = interceptedType.FullName;
			methods[getEntityNameMethod] = invocation => invocation.ReturnValue = typeName;
			methods[getPersistentClassMethod] = invocation => invocation.ReturnValue = interceptedType;
			
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
				if (attribute == null || attributes.Any(a => a.PersistAs != PropertyPersistenceLocation.Detail))
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
