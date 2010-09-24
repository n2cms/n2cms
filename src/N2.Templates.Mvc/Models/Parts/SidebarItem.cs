using N2.Details;
using N2.Web.UI.WebControls;
using N2.Integrity;

namespace N2.Templates.Mvc.Models.Parts
{
	[WithEditableTitle("Title", 10)]
	public abstract class SidebarItem : AbstractItem
	{
		[Displayable(typeof (H4), "Text")]
		public override string Title
		{
			get { return base.Title; }
			set { base.Title = value; }
		}
	}
}