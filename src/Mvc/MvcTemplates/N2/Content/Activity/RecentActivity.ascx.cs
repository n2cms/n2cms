using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Edit.Web;
using N2.Persistence;
using N2.Management.Activity;
using N2.Edit.Activity;
using N2.Edit;
using N2.Web;
using N2.Engine;
using N2.Edit.Versioning;
using N2.Web.UI.WebControls;

namespace N2.Management.Content.Activity
{
    [KeepAliveControlPanel]
    public partial class RecentActivity : EditUserControl
    {
        //protected override void OnInit(EventArgs e)
        //{
        //    base.OnInit(e);
        //    CurrentItem = Selection.SelectedItem;
        //    Visible = Engine.Config.Sections.Management.Collaboration.ActivityTrackingEnabled;
        //}

        //protected override void  OnDataBinding(EventArgs e)
        //{
	       // try
	       // {
		      //  var allVersions = Engine.Resolve<IVersionManager>()
			     //   .GetVersionsOf(CurrentItem.VersionOf.Value ?? CurrentItem, skip: 0, take: 4);

		      //  var activities = ManagementActivity.GetActivity(Engine, CurrentItem);
		      //  ActivitiesJson = ManagementActivity.ToJson(activities);
		      //  ShowActivities = activities.Count > 0;

		      //  base.OnDataBinding(e);
	       // }
	       // catch (Exception ex)
	       // {
		      //  Logger.Error(ex);
		      //  activityTemplatePlaceholder.Visible = false;
		      //  errorDisplay.Visible = true;
		      //  errorDisplayText.Text = ex.ToString();
	       // }
        //}

        public ContentItem CurrentItem { get; set; }

        //public string ActivitiesJson { get; set; }

        //public bool ShowActivities { get; set; }
    }
}
