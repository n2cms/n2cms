using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Collections.Generic;
using N2.Web;

namespace N2.Edit.Versions
{
	[ToolbarPlugin("", "versions", "~/Edit/Versions/Default.aspx?selected={selected}", ToolbarArea.Preview, Targets.Preview,  "~/Edit/Versions/Img/book_previous.gif", 90, ToolTip = "versions", GlobalResourceClassName = "Toolbar")]
	public partial class Default : Web.EditPage
	{
		ContentItem publishedItem;

		Persistence.IPersister persister;
		Persistence.IVersionManager versioner;

		protected override void OnInit(EventArgs e)
		{
            hlCancel.NavigateUrl = CancelUrl();

            bool isVersionable = SelectedItem.GetType().GetCustomAttributes(typeof(Persistence.NotVersionableAttribute), true).Length == 0;
            cvVersionable.IsValid = isVersionable;
			
			persister = N2.Context.Persister;
			versioner = N2.Context.Current.Resolve<Persistence.IVersionManager>();

			publishedItem = SelectedItem.VersionOf ?? SelectedItem;

			base.OnInit(e);
		}

		protected void gvHistory_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			ContentItem currentVersion = SelectedItem;
			int id = Convert.ToInt32(e.CommandArgument);
			if (currentVersion.ID == id)
			{
				// do nothing
			}
			else if (e.CommandName == "Publish")
			{
				N2.ContentItem previousVersion = Engine.Persister.Get(id);
				bool deletePrevious = previousVersion.Updated > currentVersion.Updated;
				versioner.ReplaceVersion(currentVersion, previousVersion);
				if (deletePrevious)
					persister.Delete(previousVersion);

				Refresh(currentVersion, ToolbarArea.Navigation);
				this.DataBind();
			}
			else if (e.CommandName == "Delete")
			{
				ContentItem item = Engine.Persister.Get(id);
				persister.Delete(item);
			}
		}

		protected void gvHistory_RowDeleting(object sender, GridViewDeleteEventArgs e)
		{

		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			IList<ContentItem> versions = Find.Items
				.Where.VersionOf.Eq(publishedItem)
				.Or.ID.Eq(publishedItem.ID)
				.OrderBy.Updated.Desc.Select();

			gvHistory.DataSource = versions;
			gvHistory.DataBind();
		}

		protected override string GetPreviewUrl(ContentItem item)
		{
			if (item.VersionOf == null)
				return item.Url;

			return Url.Parse(item.FindTemplate(TemplateData.DefaultAction).RewrittenUrl).AppendQuery("preview", item.ID).AppendQuery("previewOf", item.VersionOf.ID);
		}

		protected bool IsPublished(object dataItem)
		{
			return publishedItem == dataItem;
		}
	}
}
