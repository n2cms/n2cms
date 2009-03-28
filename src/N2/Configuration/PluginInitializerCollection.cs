using System.Xml;

namespace N2.Configuration
{
	public class PluginInitializerCollection : LazyRemovableCollection<PluginInitializerElement>
	{
		protected override void OnDeserializeElement(PluginInitializerElement element, XmlReader reader)
		{
			element.Type = reader.GetAttribute("type");
		}
	}
}