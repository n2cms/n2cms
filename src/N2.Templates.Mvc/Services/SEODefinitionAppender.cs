using Castle.Core;
using N2.Definitions;
using N2.Details;
using N2.Templates.Mvc.Items;
using N2.Web.UI;
using N2.Plugin;

namespace N2.Templates.Mvc.Services
{
	public class SEODefinitionAppender : IAutoStart
	{
		private readonly IDefinitionManager definitions;
		private string titleTitle = "Alternative title";
		private string metaKeywordsTitle = "Keywords";
		private string metaDescriptionTitle = "Description";
		private int sortIndex = 200;

		public SEODefinitionAppender(IDefinitionManager definitions)
		{
			this.definitions = definitions;
		}

		public string TitleTitle
		{
			get { return titleTitle; }
			set { titleTitle = value; }
		}

		public string MetaKeywordsTitle
		{
			get { return metaKeywordsTitle; }
			set { metaKeywordsTitle = value; }
		}

		public string MetaDescriptionTitle
		{
			get { return metaDescriptionTitle; }
			set { metaDescriptionTitle = value; }
		}

		public int SortIndex
		{
			get { return sortIndex; }
			set { sortIndex = value; }
		}

		#region IStartable Members

		public void Start()
		{
			foreach (ItemDefinition definition in definitions.GetDefinitions())
			{
				if(IsPage(definition))
				{
					SeparatorAttribute separator = new SeparatorAttribute("seo", SortIndex);
					separator.ContainerName = Tabs.Details;
					definition.Add(separator);

					var titleEditor = AddEditableText(definition, TitleTitle, TitleAndMetaTagApplyer.HeadTitle, SortIndex + 3, 200);
					titleEditor.HelpTitle = "This text is displayed in the browser's title bar and in search engine result lists, when this value is empty the page title is used.";
					var keywordsEditor = AddEditableText(definition, MetaKeywordsTitle, TitleAndMetaTagApplyer.MetaKeywords, SortIndex + 6, 400);
					keywordsEditor.HelpTitle = "Keywords that may be used by search engines to pinpoint whe subject of this page. Text content and incoming links also affect this.";
					var descriptionEditor = AddEditableText(definition, MetaDescriptionTitle, TitleAndMetaTagApplyer.MetaDescription, SortIndex + 9, 1000);
					descriptionEditor.HelpTitle = "A text that can be used by search engines to describe this page when displaying it in search results.";
				}
			}
		}

		private EditableTextBoxAttribute AddEditableText(ItemDefinition definition, string title, string name, int sortOrder, int maxLength)
		{
			EditableTextBoxAttribute editor = new EditableTextBoxAttribute(title, sortOrder, maxLength);
			editor.Name = name;
			editor.ContainerName = Tabs.Details;
			definition.Add(editor);
			return editor;
		}

		private bool IsPage(ItemDefinition definition)
		{
			return typeof (AbstractPage).IsAssignableFrom(definition.ItemType);
		}

		public void Stop()
		{
		}

		#endregion
	}
}