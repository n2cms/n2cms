namespace N2.Configuration
{
	/// <summary>
	/// A configurable collection of interface plugins.
	/// </summary>
	public class InterfacePluginCollection : LazyRemovableCollection<InterfacePluginElement>
	{
		protected override void OnDeserializeRemoveElement(InterfacePluginElement element, System.Xml.XmlReader reader)
		{
			element.Type = reader.GetAttribute("type");
		}
	}
}