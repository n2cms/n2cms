using N2.Integrity;
using N2.Templates.Items;

namespace N2.Templates.News.Items
{
	[Definition("News Container", "NewsContainer", "A list of news. News items can be added to this page.", "", 150)]
	[RestrictParents(typeof (IStructuralPage))]
	public class NewsContainer : AbstractContentPage
	{
		public override string IconUrl
		{
			get { return "~/News/UI/Img/newspaper_link.png"; }
		}

		public override string TemplateUrl
		{
			get { return "~/News/UI/Container.aspx"; }
		}
	}
}