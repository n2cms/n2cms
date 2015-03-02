using System.Collections.Generic;
using System.Linq;
using N2.Collections;
using N2.Integrity;
using N2.Web.Mvc;
using N2.Definitions;

namespace N2.Templates.Mvc.Models.Pages
{
    [PageDefinition("News Container",
        Description = "A list of news. News items can be added to this page.",
        SortOrder = 150,
        IconClass = "fa fa-list blue")]
    [RestrictParents(typeof (IStructuralPage))]
    [SortChildren(SortBy.PublishedDescending)]
    [GroupChildren(GroupChildrenMode.PublishedYear)]
    public class NewsContainer : ContentPageBase
    {
        public IList<News> NewsItems
        {
            get { return Children.OfType<News>().ToList(); }
        }
    }
}
