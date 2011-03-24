using System;
using System.Web.UI;
using N2.Persistence.Finder;
using N2.Resources;

namespace N2.Edit.Navigation
{
    [SearchControl]
	public partial class Search : NavigationPage
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			Register.JQueryUi(Page);

			string query = Request.QueryString["query"];
            if (string.IsNullOrEmpty(query))
            {
                Response.Redirect("Tree.aspx");
            }
            else
            {
                idsItems.Query = CreateQuery(query);
                dgrItems.DataBind();
            }
		}

		private IQueryEnding CreateQuery(string searchQuery)
		{
			string likeQuery = "%" + searchQuery + "%";
			IQueryAction query = Find.Items
				.Where.Name.Like(likeQuery)
				.Or.SavedBy.Like(likeQuery)
				.Or.Title.Like(likeQuery)
				.Or.Detail().Like(likeQuery);

			int id;
			if (Int32.TryParse(searchQuery, out id))
				query = query.Or.ID.Eq(id);

			return query.MaxResults(1000)
                .OrderBy.Updated.Desc
                .Filters(Engine.EditManager.GetEditorFilter(Page.User));
		}
	}
}
