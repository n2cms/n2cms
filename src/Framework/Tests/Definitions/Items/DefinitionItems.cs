using N2.Definitions;
using N2.Integrity;
using N2.Details;
using N2.Web.UI;
using N2.Persistence;

[assembly: RemoveEditable("Text", typeof(N2.Tests.Definitions.Items.DefinitionRemovable))]

namespace N2.Tests.Definitions.Items
{
    [PageDefinition]
    [RestrictChildren(typeof(ChildWithCardinality))]
    public class RestrictsChildWithCardinality : N2.ContentItem
    {
    }

    [PageDefinition]
    [RestrictCardinality]
    public class ChildWithCardinality : N2.ContentItem
    {
    }

	[PageDefinition(AuthorizedRoles = new string[0])]
	public class DefinitionAutoCreatedItem : N2.ContentItem
	{
	}

	[PageDefinition]
	[RestrictChildren(typeof(DefinitionOppressedChild))]
	public class DefinitionControllingParent : ContentItem
	{
	}

	[PartDefinition]
	[AllowedZones(AllowedZones.AllNamed)]
	public class DefinitionFreeItem : N2.ContentItem
	{
	}

	[PartDefinition]
	[RestrictParents(typeof(ILeftColumnlPage))]
	[AllowedZones(AllowedZones.All)]
	public class DefinitionMenuItem : N2.ContentItem
	{
	}

	[Definition]
	[N2.Integrity.RestrictParents(typeof(DefinitionNewsPage))]
	[N2.Integrity.AllowedZones("", "Right")]
	public class DefinitionNewsList : DefinitionRightColumnPart
	{
		public override bool IsPage
		{
			get
			{
				return string.IsNullOrEmpty(ZoneName) ? true : false;
			}
		}
	}

	[PageDefinition]
	[N2.Web.UI.FieldSetContainer("specific", "News specific", 100)]
	public class DefinitionNewsPage : DefinitionTextPage
	{
		[EditableText("Plain Text", 200)]
		public override string Text
		{
			get { return base.Text; }
			set { base.Text = value; }
		}
	}

	[PageDefinition]
	public class DefinitionOne : N2.ContentItem
	{
	}

	[PageDefinition]
	public class DefinitionOppressedChild : ContentItem
	{
	}

	[PartDefinition]
	public class DefinitionPartDefinedItem : ContentItem
	{
	}

	[PageDefinition]
	public class DefinitionRemovable : ContentItem
	{
		[EditableText]
		public string Description { get; set; }

		[EditableFreeTextArea]
		public string Text { get; set; }
	}

	[PageDefinition]
	[RemoveEditable("Description")]
	public class DefinitionRemoves : DefinitionRemovable
	{
	}

	[PageDefinition]
	[RemoveDefinitions(typeof(DefinitionTwo))]
	public class DefinitionRemovesNumber2 : N2.ContentItem
	{
	}

	[PageDefinition]
	public class DefinitionDisabled : N2.ContentItem
	{
	}

	[PageDefinition]
	public class DefinitionReplaced : N2.ContentItem
	{
	}

	[PageDefinition]
	[ReplacesParentDefinition(ReplacementMode = DefinitionReplacementMode.Disable)]
	public class DefinitionDisablingReplacement : DefinitionDisabled
	{
	}
	
	[PageDefinition]
	[ReplacesParentDefinition]
	public class DefinitionReplacement : DefinitionReplaced
	{
	}

	[PageDefinition]
	[RemoveDefinitions(DefinitionReplacementMode.Disable, typeof(DefinitionOne))]
	public class DefinitionReplacesNumber1 : N2.ContentItem
	{
	}

	[PartDefinition]
	[AllowedZones("Right")]
	public abstract class DefinitionRightColumnPart : N2.ContentItem
	{
		public override bool IsPage
		{
			get { return false; }
		}
	}

	[PartDefinition]
	public class DefinitionRightColumnTeaser : DefinitionRightColumnPart
	{
	}

	[PageDefinition]
	[AvailableZone("Left and Center", "LeftAndCenter")]
	[RestrictParents(AllowedTypes.None)]
	public class DefinitionStartPage : DefinitionTwoColumnPage, ILeftColumnlPage
	{
	}

	[PartDefinition]
	[AllowedZones("Right", "LeftAndCenter")]
	[RestrictParents(typeof(DefinitionTwoColumnPage))]
	public class DefinitionTextItem : DefinitionRightColumnPart
	{
	}

	[PageDefinition("A text page")]
	[N2.Web.UI.FieldSetContainer("specific", "Specific", 20, ContainerName = "general")]
	public class DefinitionTextPage : DefinitionTwoColumnPage, ILeftColumnlPage
	{
		[EditableFreeTextArea("Text", 100, ContainerName = "specific")]
		[DisplayableLiteral]
		public virtual string Text
		{
			get { return (string)(GetDetail("Text") ?? string.Empty); }
			set { SetDetail("Text", value); }
		}

	}

	[PageDefinition]
	public class DefinitionTwo : N2.ContentItem
	{
	}

	[PageDefinition]
	[AvailableZone("Right", "Right")]
	[WithEditableTitle("Title", 0, ContainerName = "general")]
	[WithEditableName("Name", 1)]
	[FieldSetContainer("general", "General", 10)]
	public abstract class DefinitionTwoColumnPage : ContentItem
	{
	}

	// is not decorated with [Definition]
	public class DefinitionUndefined : N2.ContentItem
	{
	}

	public interface ILeftColumnlPage
	{
	}

	[PageDefinition]
	public class DefinitionWithPersistable : N2.ContentItem
	{
		[Persistable]
		public virtual string Hello { get; set; }
	}

}
