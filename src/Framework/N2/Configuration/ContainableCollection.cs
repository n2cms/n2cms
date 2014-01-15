using System.Configuration;
using System.Xml;

namespace N2.Configuration
{
    /// <summary>
    /// Represents a collection of containable elements.
    /// </summary>
    [ConfigurationCollection(typeof(ContainableElement))]
    public class ContainableCollection : LazyRemovableCollection<ContainableElement>
    {
        public static ContainableCollection Deserialize(XmlReader reader)
        {
            ContainableCollection collection = new ContainableCollection();
            collection.DeserializeElement(reader, true);
            return collection;
        }
    }
}
