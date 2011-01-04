using N2.Details;

namespace N2.Tests.Definitions.Items
{
	[PageDefinition("A text page")]
	[N2.Web.UI.FieldSetContainer("specific", "Specific", 20, ContainerName = "general")]
	public class DefinitionTextPage : DefinitionTwoColumnPage, ILeftColumnlPage
	{
		[EditableFreeTextArea("Text", 100, ContainerName = "specific")]	
		public virtual string Text
		{
			get { return (string)(GetDetail("Text") ?? string.Empty); }
			set { SetDetail("Text", value); }
		}

	}
}
