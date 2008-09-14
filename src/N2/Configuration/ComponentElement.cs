using System.Configuration;

namespace N2.Configuration
{
    /// <summary>
    /// A service definition to add to the N2 container. This can be used to replace core services.
    /// </summary>
    public class ComponentElement : ConfigurationElement
    {
        /// <summary>Class and name and assembly of the service interface, e.g. "MyNamespace.MyInterface, MyAssembly"</summary>
        [ConfigurationProperty("service")]
        public string Service
        {
            get { return (string)base["service"]; }
            set { base["service"] = value; }
        }

        /// <summary>Class and name and assembly of the implementation to add, e.g. "MyNamespace.MyClass, MyAssembly". If no service is defined the class itself will represent the service.</summary>
        [ConfigurationProperty("implementation", IsRequired = true)]
        public string Implementation
        {
            get { return (string)base["implementation"]; }
            set { base["implementation"] = value; }
        }
    }
}
