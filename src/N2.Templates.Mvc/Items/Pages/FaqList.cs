using N2.Integrity;
using System.Collections.Generic;
using N2.Templates.Mvc.Items.Items;
using N2.Web.Mvc;

namespace N2.Templates.Mvc.Items.Pages
{
	[PageDefinition("Frequently Asked Questions",
		Description = "A list of frequently asked questions with answers.",
		SortOrder = 200,
		IconUrl = "~/Content/Img/help.png")]
	[AvailableZone("Questions", "Questions")]
	[RestrictParents(typeof (IStructuralPage))]
	[MvcConventionTemplate]
	public class FaqList : AbstractContentPage, IStructuralPage
	{
		[N2.Details.EditableChildren("Questions", "Questions", 110, ContainerName = Tabs.Content)]
		public virtual IList<Faq> Questions
		{
			get { return GetChildren<Faq>("Questions"); }
		}
	}
}