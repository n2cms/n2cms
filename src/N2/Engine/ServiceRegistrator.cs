using System;
using N2.Plugin;
using Castle.Core;
using System.Collections.Generic;

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
			foreach (var info in services)
			{
				Type serviceType = info.Attribute.ServiceType ?? info.DecoratedType;
				container.AddComponent(info.Attribute.Key ?? info.DecoratedType.FullName, serviceType, info.DecoratedType);
			}
		}
    }
}