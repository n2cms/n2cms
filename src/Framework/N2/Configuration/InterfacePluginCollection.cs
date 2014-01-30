using System.Configuration;

namespace N2.Configuration
{
    /// <summary>
    /// A configurable collection of interface plugins.
    /// </summary>
    [ConfigurationCollection(typeof(InterfacePluginElement))]
    public class InterfacePluginCollection : LazyRemovableCollection<InterfacePluginElement>
    {
        protected override void OnDeserializeRemoveElement(InterfacePluginElement element, System.Xml.XmlReader reader)
        {
            element.Type = reader.GetAttribute("type");
        }
    }
}
