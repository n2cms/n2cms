using System;
using N2.Collections;
using System.Collections.Generic;
using RootPage=N2.Templates.Items.RootPage;

namespace N2.Templates.UI.Parts
{
	public partial class Statistics : Web.UI.TemplateUserControl<RootPage, Templates.Items.Statistics>
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