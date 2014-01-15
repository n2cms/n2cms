using System;
using N2.Engine;

namespace N2.Web
{
    /// <summary>
    /// Used in combination with the [ConventionTemplate] attribute to
    /// point out the location of content item templates. All content
    /// item types with a ConventionTemplate attribute in the same 
    /// assembly as this attribute are affected by this optin.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class ConventionTemplateDirectoryAttribute : Attribute
    {
        string virtualDirectory;

        public ConventionTemplateDirectoryAttribute(string virtualDirectory)
        {
            this.virtualDirectory = virtualDirectory;
        }

        public string VirtualDirectory
        {
            get { return virtualDirectory; }
        }

        /// <summary>
        /// Gets the directory specified as convention for the type. By default this is
        /// the directory specified by a ConventionTemplateDirectoryAttribute in the 
        /// same assembly as the supplied type, or "~/UI/Views/" if no such attribute is 
        /// found.
        /// </summary>
        /// <param name="itemType">The type of item whose convention directory to get.</param>
        /// <returns>The virtual directory path for the specified item type.</returns>
        public static string GetDirectory(Type itemType)
        {
            if (itemType == null) throw new ArgumentNullException("itemType");

            ConventionTemplateDirectoryAttribute locationAttribute;
            if (SingletonDictionary<Type, ConventionTemplateDirectoryAttribute>.Instance.TryGetValue(itemType, out locationAttribute))
                return locationAttribute.VirtualDirectory;

            object[] directoryAttributes = itemType.Assembly.GetCustomAttributes(typeof(ConventionTemplateDirectoryAttribute), false);
            if(directoryAttributes.Length > 0)
                SetDirectoryConvention(itemType, directoryAttributes[0] as ConventionTemplateDirectoryAttribute);
            else
                SetDirectoryConvention(itemType, new ConventionTemplateDirectoryAttribute("~/UI/Views/"));
            
            return GetDirectory(itemType);
        }

        public static void SetDirectoryConvention(Type itemType, ConventionTemplateDirectoryAttribute instance)
        {
            if (itemType == null) throw new ArgumentNullException("itemType");

            if (string.IsNullOrEmpty(instance.virtualDirectory))
                instance.virtualDirectory = "~/";
            else if (!instance.virtualDirectory.EndsWith("/"))
                instance.virtualDirectory += "/";

            SingletonDictionary<Type, ConventionTemplateDirectoryAttribute>.Instance[itemType] = instance;
        }
    }
}
