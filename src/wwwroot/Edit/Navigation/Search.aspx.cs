using System;
using System.Web.UI;
using System.Text.RegularExpressions;
using N2.Persistence.Finder;

namespace N2.Edit.Navigation
{
	[ToolbarPlugin("", "search", "navigation/search.aspx?selected={selected}", ToolbarArea.Navigation, "navigation", "~/Edit/Img/Ico/page_find.gif", -15, ToolTip = "search", GlobalResourceClassName = "Toolbar")]
	public partial class Search : NavigationPage
	{
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			this.txtQuery.Focus();
		}

		protected void btnSerach_Click(object sender, ImageClickEventArgs e)
		{
			this.idsItems.Query = CreateQuery();
			this.dgrItems.DataBind();
		}

		private IQueryEnding CreateQuery()
		{
			string likeQuery = "%" + this.txtQuery.Text + "%";
			N2.Persistence.Finder.IQueryAction query = N2.Find.Items
				.Where.Name.Like(likeQuery)
				.Or.SavedBy.Like(likeQuery)
				.Or.Title.Like(likeQuery)
				.Or.Detail().Like(likeQuery);

			if (Regex.IsMatch(this.txtQuery.Text, @"^\d+$", RegexOptions.Compiled))
				query = query.Or.ID.Eq(int.Parse(this.txtQuery.Text));

			return query.Filters(Engine.EditManager.GetEditorFilter(Page.User));
		}
	}
}
