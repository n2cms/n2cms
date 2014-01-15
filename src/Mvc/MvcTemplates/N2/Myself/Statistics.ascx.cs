using System;
using System.Collections.Generic;
using N2.Collections;
using N2.Details;
using N2.Web.UI;
using N2.Edit.Versioning;
using N2.Persistence;

namespace N2.Management.Myself
{
    [PartDefinition("Statistics", Name = "Statistics", 
        TemplateUrl = "{ManagementUrl}/Myself/Statistics.ascx",
        IconUrl = "{ManagementUrl}/Resources/icons/information.png")]
    [WithEditableTitle("Title", 10)]
    public class StatisticsPart : RootPartBase
    {
        public override string Title
        {
            get { return base.Title ?? "Statistics"; }
            set { base.Title = value; }
        }
    }

    public partial class Statistics : ContentUserControl<ContentItem, StatisticsPart>
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            IList<ContentItem> items = Find.Items.All.Select();
            int itemsCount = items.Count;
            lblItems.Text = itemsCount.ToString();
            
            PageFilter.FilterPages(items);
            lblPages.Text = items.Count.ToString();

            long totalCount = itemsCount + Engine.Resolve<ContentVersionRepository>().Repository.Count();
                //Find.Items.All.PreviousVersions(N2.Persistence.Finder.VersionOption.Include).Select().Count;
            lblVersionsRatio.Text = string.Format("{0:F2}", (double)totalCount / (double)itemsCount);

            lblServed.Text = "unknown";
            lblChangesLastWeek.Text = Engine.Persister.Repository.Count(Parameter.LessOrEqual("Updated", N2.Utility.CurrentTime().AddDays(-7))).ToString();
                //Find.Items.Where.Updated.Ge(N2.Utility.CurrentTime().AddDays(-7)).Select().Count.ToString();

            DataBind();
        }
    }
}
