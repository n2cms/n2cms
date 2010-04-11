using System.Web.UI.WebControls;
using N2.Details;
using N2.Integrity;
using N2.Templates.Mvc.Services;
using N2.Web.Mvc;

namespace N2.Templates.Mvc.Models.Pages
{
	[PageDefinition("News", Description = "A news page.", SortOrder = 155,
		IconUrl = "~/Content/Img/newspaper.png")]
	[RestrictParents(typeof (NewsContainer))]
	public class News : AbstractContentPage, ISyndicatable
	{
		public News()
		{
			Visible = false;
		}

		[EditableTextBox("Introduction", 90, ContainerName = Tabs.Content, TextMode = TextBoxMode.MultiLine, Rows = 4,
			Columns = 80)]
		public virtual string Introduction
		{
			get { return (string) (GetDetail("Introduction") ?? string.Empty); }
			set { SetDetail("Introduction", value, string.Empty); }
		}

		string ISyndicatable.Summary
		{
			get { return Introduction; }
		}
	}
}