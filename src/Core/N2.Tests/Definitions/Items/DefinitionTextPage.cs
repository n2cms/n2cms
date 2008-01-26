namespace N2.Tests.Definitions.Items
{
	[Definition("A text page")]
	[N2.Web.UI.FieldSet("specific", "Specific", 20, ContainerName = "general")]
	public class DefinitionTextPage : DefinitionTwoColumnPage, ILeftColumnlPage
	{
		[Details.EditableFreeTextArea("Text", 100, ContainerName = "specific")]	
		public virtual string Text
		{
			get { return (string)(GetDetail("Text") ?? string.Empty); }
			set { SetDetail("Text", value); }
		}

	}
}
