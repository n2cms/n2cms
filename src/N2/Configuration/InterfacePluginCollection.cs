namespace N2.Configuration
{
	public class InterfacePluginCollection : LazyRemovableCollection<InterfacePluginElement>
	{
		protected override void OnDeserializeElement(InterfacePluginElement element, System.Xml.XmlReader reader)
		{
			element.Type = reader.GetAttribute("type");
		}
	}
}