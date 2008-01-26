using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using N2.Collections;
using System.Collections.Generic;
using N2.Templates.Web;

namespace N2.Templates.UI.Parts
{
	public partial class Statistics : Web.UI.TemplateUserControl<Items.RootPage, Items.RootParts.Statistics>
	{
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			IList<ContentItem> items = Find.Items.All.Select();
			int itemsCount = items.Count;
			lblItems.Text = itemsCount.ToString();
			
			PageFilter.FilterPages(items);
			lblPages.Text = items.Count.ToString();

			int totalCount = Find.Items.All.PreviousVersions(N2.Persistence.Finder.VersionOption.Include).Select().Count;
			lblVersionsRatio.Text = string.Format("{0:F2}", (double)totalCount / (double)itemsCount);

			lblServed.Text = "unknown";
			lblChangesLastWeek.Text = Find.Items.Where.Updated.Ge(DateTime.Now.AddDays(-7)).Select().Count.ToString();
			rptLatestChanges.DataSource = Find.Items.All.MaxResults(CurrentItem.LatestChangesMaxCount).OrderBy.Updated.Desc.Select();

			DataBind();
		}
	}
}