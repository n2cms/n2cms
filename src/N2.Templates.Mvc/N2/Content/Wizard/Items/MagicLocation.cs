using N2.Definitions;
using N2.Details;
using N2.Edit.Wizard.Details;
using N2.Integrity;
using N2.Installation;
using N2.Edit.Trash;

namespace N2.Edit.Wizard.Items
{
	[PartDefinition("Magic Location",
		IconUrl = "|Management|/Resources/icons/wand.png")]
	[RestrictParents(typeof(Wonderland))]
	[WithEditableTitle("Title", 10)]
    [NotThrowable]
    public class MagicLocation : ContentItem
	{
		[EditableLink("Location", 100)]
		public virtual ContentItem Location
		{
			get { return (ContentItem)GetDetail("Location"); }
			set { SetDetail("Location", value); }
		}

		[EditableDefinition("Definition", 110)]
		public virtual string ItemDiscriminator
		{
			get { return (string)GetDetail("ItemDiscriminator"); }
			set { SetDetail("ItemDiscriminator", value); }
		}

		[EditableTextBox("Content template", 110)]
		public virtual string ContentTemplate
		{
			get { return (string)GetDetail("ContentTemplate"); }
			set { SetDetail("ContentTemplate", value); }
		}

		[EditableTextBox("Zone name", 120)]
		public virtual string ItemZoneName
		{
			get { return (string)GetDetail("ItemZoneName"); }
			set { SetDetail("ItemZoneName", value); }
		}

		[EditableUrl("Icon", 130)]
		public virtual string Icon
		{
			get
			{
				return (string) GetDetail("Icon") 
					?? GetDefinitionIcon();
			}
			set { SetDetail("Icon", value, string.Empty); }
		}

		private string GetDefinitionIcon()
		{
			ItemDefinition definition = GetDefinition(Context.Definitions);
			if(definition != null)
				return definition.IconUrl;
			return null;
		}

		[EditableTextBox("Tooltip", 140)]
		public virtual string ToolTip
		{	
			get { return (string)(GetDetail("ToolTip") ?? string.Empty); }
			set { SetDetail("ToolTip", value, string.Empty); }
		}

		[EditableFreeTextArea("Description", 150)]
		public virtual string Description
		{
			get { return (string)(GetDetail("Description") ?? string.Empty); }
			set { SetDetail("Description", value, string.Empty); }
		}

		public virtual ItemDefinition GetDefinition(IDefinitionManager definitions)
		{
			foreach (ItemDefinition definition in definitions.GetDefinitions())
				if (definition.Discriminator == ItemDiscriminator)
					return definition;
			return null;
		}

		public override bool IsPage
		{
			get { return false; }
		}

		public override string IconUrl
		{
			get
			{
				return Icon
					?? "|Management|/Resources/icons/wand.png";
			}
		}
	}
}
