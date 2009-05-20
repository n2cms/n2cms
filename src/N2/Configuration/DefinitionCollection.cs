namespace N2.Configuration
{
	/// <summary>
	/// A collection of configured item definitions that supports remove and add.
	/// </summary>
	public class DefinitionCollection : LazyRemovableCollection<DefinitionElement>
	{
		protected override void OnDeserializeElement(DefinitionElement element, System.Xml.XmlReader reader)
		{
			element.Description = reader.GetAttribute("description");
			element.SortOrder = int.Parse(reader.GetAttribute("sortOrder"));
			element.Title = reader.GetAttribute("title");
			element.ToolTip = reader.GetAttribute("toolTip");
			element.Type = reader.GetAttribute("type");
		}
	}
}