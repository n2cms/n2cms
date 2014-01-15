using System.Configuration;
using System.Xml;

namespace N2.Configuration
{
    /// <summary>
    /// A configurable collection of plugin initializers.
    /// </summary>
    [ConfigurationCollection(typeof(PluginInitializerElement))]
    public class PluginInitializerCollection : LazyRemovableCollection<PluginInitializerElement>
    {
        protected override void OnDeserializeRemoveElement(PluginInitializerElement element, XmlReader reader)
        {
            element.Type = reader.GetAttribute("type");
        }
    }
}
