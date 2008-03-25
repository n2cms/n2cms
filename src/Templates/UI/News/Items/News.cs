using System.Collections.Generic;
using System.Web.UI.WebControls;
using N2.Details;
using N2.Integrity;
using N2.Templates.Items;
using N2.Templates.Syndication;

namespace N2.Templates.News.Items
{
	[Definition("News", "News", "A news page.", "", 155)]
	[RestrictParents(typeof (NewsContainer))]
	public class News : AbstractContentPage, ISyndicatable
	{
		public override void AddTo(ContentItem newParent)
		{
			Utility.Insert(this, newParent, "Published DESC");
		}

		public override string IconUrl
		{
			get { return "~/News/UI/Img/newspaper.png"; }
		}

		[EditableTextBox("Introduction", 90, ContainerName = Tabs.Content, TextMode = TextBoxMode.MultiLine, Rows = 4,
			Columns = 80)]
		public virtual string Introduction
		{
			get { return (string) (GetDetail("Introduction") ?? string.Empty); }
			set { SetDetail("Introduction", value, string.Empty); }
		}

		public override string TemplateUrl
		{
			get { return "~/News/UI/Default.aspx"; }
		}

		string ISyndicatable.Summary
		{
			get { return Introduction; }
		}
	}
}