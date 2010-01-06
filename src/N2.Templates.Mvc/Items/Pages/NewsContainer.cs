using System.Collections.Generic;
using System.Linq;
using N2.Collections;
using N2.Integrity;
using N2.Web.Mvc;

namespace N2.Templates.Mvc.Items.Pages
{
	[PageDefinition("News Container",
		Description = "A list of news. News items can be added to this page.",
		SortOrder = 150,
		IconUrl = "~/Content/Img/newspaper_link.png")]
	[RestrictParents(typeof (IStructuralPage))]
	[MvcConventionTemplate("NewsList")]
	public class NewsContainer : AbstractContentPage
	{
		public IList<News> NewsItems
		{
			get { return GetChildren(new TypeFilter(typeof (News))).OfType<News>().ToList(); }
		}
	}
}