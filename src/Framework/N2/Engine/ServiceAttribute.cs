using System;

namespace N2.Engine
{
    /// <summary>
    /// Markes a service that is registered in automatically registered in N2's inversion of control container.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
    public class ServiceAttribute : Attribute
    {
        public ServiceAttribute()
        {
        }

        public ServiceAttribute(Type serviceType)
        {
            ServiceType = serviceType;
        }

        /// <summary>The type of service the attributed class represents.</summary>
        public Type ServiceType { get; protected set; }

        /// <summary>Optional key to associate with the service.</summary>
        public string Key { get; set; }

        /// <summary>Configurations for which this service is registered.</summary>
        public string Configuration { get; set; }

        /// <summary>A static accessor property used to retrieve the service instance instead of instiatating it.</summary>
        public string StaticAccessor { get; set; }

        /// <summary>A service defined by a <see cref="ServiceAttribute"/> to replace with the attributed implementation.</summary>
        public Type Replaces { get; set; }
    }
}
