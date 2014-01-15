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
            return finder.Find<ServiceAttribute>(typeof(object), false)
                .Where(t => !t.Type.IsAbstract)
                .Where(t => !t.Type.IsInterface)
                .Where(t => !t.Type.IsEnum)
                .Where(t => !t.Type.IsValueType)
                .Select(ai => new AttributeInfo<ServiceAttribute> { Attribute = ai.Attribute, DecoratedType = ai.Type });
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
                string key = info.Attribute.Key ?? (info.Attribute.ServiceType ?? info.DecoratedType).FullName + "->" + info.DecoratedType.FullName;
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

        public virtual IEnumerable<AttributeInfo<ServiceAttribute>> FilterServices(IEnumerable<AttributeInfo<ServiceAttribute>> services, params string[] configurationKeys)
        {
            return services.Where(s => s.Attribute.Configuration == null || configurationKeys.Contains(s.Attribute.Configuration));
        }

        public virtual IEnumerable<AttributeInfo<ServiceAttribute>> FilterServices(IEnumerable<AttributeInfo<ServiceAttribute>> services, IEnumerable<Type> skipTypes)
        {
            return services.Where(s => !skipTypes.Contains(s.Attribute.ServiceType ?? s.DecoratedType));
        }
    }
}
