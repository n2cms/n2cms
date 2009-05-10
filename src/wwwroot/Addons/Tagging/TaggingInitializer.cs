using N2.Addons.Tagging.Details;
using N2.Definitions;
using N2.Engine;
using N2.Plugin;
using N2.Templates.Items;
using N2.Templates;

namespace N2.Addons.Tagging
{
	/// <summary>
	/// Registers components and services needed by the tagging addon.
	/// </summary>
	[AutoInitialize]
	public class TaggingInitializer : IPluginInitializer
	{
		public void Initialize(IEngine engine)
		{
			foreach (ItemDefinition definition in engine.Definitions.GetDefinitions())
			{
				if (IsContentPage(definition))
				{
					var tagEditable = new EditableTagsAttribute();
					tagEditable.Name = "Tags";
					tagEditable.ContainerName = Tabs.Content;
					tagEditable.SortOrder = 200;
					definition.Add(tagEditable);
				}
			}
		}

		private bool IsContentPage(ItemDefinition definition)
		{
			return typeof(AbstractContentPage).IsAssignableFrom(definition.ItemType);
		}
	}
}