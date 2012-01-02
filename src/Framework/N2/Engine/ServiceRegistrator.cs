using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace N2.Engine
{
    /// <summary>
    /// Registers service in the N2 inversion of container upon start.
    /// </summary>
    public class ServiceRegistrator
    {
        ITypeFinder finder;
        IServiceContainer container;

        public ServiceRegistrator(ITypeFinder finder, IServiceContainer container)
        {
            this.finder = finder;
            this.container = container;
        }

		public virtual IEnumerable<AttributeInfo<ServiceAttribute>> FindServices()
		{
			foreach (Type type in finder.Find(typeof(object)))
			{
				var attributes = type.GetCustomAttributes(typeof(ServiceAttribute), false);
				foreach (ServiceAttribute attribute in attributes)
				{
					yield return new AttributeInfo<ServiceAttribute> { Attribute = attribute, DecoratedType = type };
				}
			}
		}

		public virtual void RegisterServices(IEnumerable<AttributeInfo<ServiceAttribute>> services)
		{
			var allServices = services.ToList();
			var replacementServices = allServices
				.Where(s => s.Attribute.Replaces != null)
				.Select(s => s.Attribute.Replaces).ToList();
			var addedServices = allServices
				.Where(s => !replacementServices.Contains(s.DecoratedType))
				.Where(s => s.Attribute.Replaces != null || s.Attribute.ServiceType == null || !replacementServices.Contains(s.Attribute.ServiceType));
			foreach (var info in addedServices)
			{
				Type serviceType = info.Attribute.ServiceType ?? info.DecoratedType;
				string key = info.Attribute.Key ?? CalculateName(info);
				if(string.IsNullOrEmpty(info.Attribute.StaticAccessor))
					container.AddComponent(key, serviceType, info.DecoratedType);
				else
				{
					var pi = info.DecoratedType.GetProperty(info.Attribute.StaticAccessor, BindingFlags.Public | BindingFlags.Static);
					if(pi == null) throw new InvalidOperationException("[Service(StaticAccessor = \"" + info.Attribute.StaticAccessor + "\")] on " + info.DecoratedType + " doesn't match an existing static property on that type. Add a static property or remove the static accessor declaration.");
					var instance = pi.GetValue(null, null);
					if(instance == null) new InvalidOperationException("[Service(StaticAccessor = \"" + info.Attribute.StaticAccessor + "\")] on " + info.DecoratedType + " defines a property that returned null. Make sure this static property returns a value.");
					if(!serviceType.IsAssignableFrom(instance.GetType())) new InvalidOperationException("[Service(StaticAccessor = \"" + info.Attribute.StaticAccessor + "\")] on " + info.DecoratedType + " defines a property that returned an invalid type. The returned object must be assignable to " + serviceType);
					container.AddComponentInstance(key, serviceType, instance);
				}
			}
		}

		private static string CalculateName(AttributeInfo<ServiceAttribute> info)
		{
			return info.Attribute.ServiceType != null && info.DecoratedType != info.Attribute.ServiceType
				? info.Attribute.ServiceType.FullName + "-" + info.DecoratedType.Name
				: info.DecoratedType.FullName;
		}

		public virtual IEnumerable<AttributeInfo<ServiceAttribute>> FilterServices(IEnumerable<AttributeInfo<ServiceAttribute>> services, params string[] configurationKeys)
		{
			return services.Where(s => s.Attribute.Configuration == null || configurationKeys.Contains(s.Attribute.Configuration));
		}
	}
}