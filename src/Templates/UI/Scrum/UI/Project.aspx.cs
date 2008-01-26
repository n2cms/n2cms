using System;
using N2.Collections;
using N2.Parts.Web.UI.WebControls;
using N2.Persistence.Finder;
using N2.Templates.Scrum.Items;

namespace N2.Templates.Scrum.UI
{
	public partial class Project : Web.UI.TemplatePage<Items.ScrumProject>
	{
		protected override void OnInit(EventArgs e)
		{
			Resources.Register.JQuery(this);
			Resources.Register.StyleSheet(this, "~/Scrum/UI/Css/Scrum.css");
			Resources.Register.JavaScript(this, "~/Scrum/UI/Js/jquery.contextmenu.r2.js");
			Resources.Register.JavaScript(this, "~/Scrum/UI/Js/n2contextmenu.js");
			Resources.Register.JavaScript(this, "~/Scrum/UI/Js/Scrum.js");
			base.OnInit(e);
		}

		protected ItemFilter[] GetFilters()
		{
			return new ItemFilter[] {
				new CountFilter(0, int.Parse(Request["count"] ?? "25"))
			};
		}

		protected IQueryAction GetQuery()
		{
			return N2.Find.Items
				.Where.Type.Eq(typeof (ScrumSprint))
				.And.Parent.Eq(CurrentPage);
		}

		protected int GetCount(string zone)
		{
			return N2.Find.Items
				.Where.Parent.Eq(CurrentItem.CurrentSprint)
				.And.ZoneName.Eq(zone).Count();
		}

		protected bool IsCurrent(object item)
		{
			return item == CurrentItem.CurrentSprint;
		}
	}
}
