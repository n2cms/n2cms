using System;
using N2.Details;
using N2.Web.UI;

namespace N2.Management.Myself
{
    [PartDefinition("Activity", 
        TemplateUrl = "{ManagementUrl}/Myself/Activity.ascx",
        IconUrl = "{ManagementUrl}/Resources/icons/text_list_numbers.png")]
    [WithEditableTitle("Title", 10)]
    public class ActivityPart : RootPartBase
    {
        public override string Title
        {
            get { return base.Title ?? "Activity"; }
            set { base.Title = value; }
        }

        [EditableNumber("Latest changes max count", 100)]
        public virtual int LatestChangesMaxCount
        {
            get { return (int)(GetDetail("LatestChangesMaxCount") ?? 5); }
            set { SetDetail("LatestChangesMaxCount", value); }
        }
        [EditableNumber("Drafts max count", 100)]
        public virtual int DraftsMaxCount
        {
            get { return (int)(GetDetail("DraftsMaxCount") ?? 5); }
            set { SetDetail("DraftsMaxCount", value); }
        }
    }

    public partial class Activity : ContentUserControl<ContentItem, ActivityPart>
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            rptLatestChanges.DataSource = Find.Items.All.MaxResults(CurrentItem.LatestChangesMaxCount).OrderBy.Updated.Desc.Select();
            rptDrafts.DataSource = Engine.Resolve<N2.Edit.Versioning.DraftRepository>().FindDrafts(0, CurrentItem.DraftsMaxCount);
            rptDrafts.ItemCommand += new System.Web.UI.WebControls.RepeaterCommandEventHandler(rptDrafts_ItemCommand);
            DataBind();
        }

        void rptDrafts_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            
        }
    }
}
