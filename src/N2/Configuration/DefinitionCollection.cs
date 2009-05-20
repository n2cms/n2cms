namespace N2.Configuration
{
	/// <summary>
	/// A collection of configured item definitions that supports remove and add.
	/// </summary>
	public class DefinitionCollection : LazyRemovableCollection<DefinitionElement>
	{
		protected override void OnDeserializeRemoveElement(DefinitionElement element, System.Xml.XmlReader reader)
		{
			element.Description = reader.GetAttribute("description");
			int sortOrder;
			if(int.TryParse(reader.GetAttribute("sortOrder"), out sortOrder))
				element.SortOrder = sortOrder;
			element.Title = reader.GetAttribute("title");
			element.ToolTip = reader.GetAttribute("toolTip");
			element.Type = reader.GetAttribute("type");
		}
	}
}