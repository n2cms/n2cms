using System;
using System.Web.UI;
using N2.Persistence.Finder;
using N2.Resources;
using N2.Persistence;
using System.Linq;
using System.Collections.Generic;

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
                dgrItems.DataSource = CreateQuery(query);
                dgrItems.DataBind();
            }
        }

        private IEnumerable<ContentItem> CreateQuery(string searchQuery)
        {
            string likeQuery = "%" + searchQuery + "%";
            //IQueryAction query = Find.Items
            //  .Where.Name.Like(likeQuery)
            //  .Or.SavedBy.Like(likeQuery)
            //  .Or.Title.Like(likeQuery)
            //  .Or.Detail().Like(likeQuery);
            ParameterCollection pc = Parameter.Like("Name", likeQuery)
                | Parameter.Like("SavedBy", likeQuery)
                | Parameter.Like("Title", likeQuery)
                | Parameter.Like(null, likeQuery).Detail();

            int id;
            if (Int32.TryParse(searchQuery, out id))
                //query = query.Or.ID.Eq(id);
                pc |= Parameter.Equal("ID", id);

            return Content.Search.Repository.Find(pc.Take(1000).OrderBy("Updated DESC"))
                .Where(Engine.EditManager.GetEditorFilter(Page.User).Match);
                //query.MaxResults(1000)
                //.OrderBy.Updated.Desc
                //.Filters(Engine.EditManager.GetEditorFilter(Page.User));
        }
    }
}
