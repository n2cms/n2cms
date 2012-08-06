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

namespace N2.Management.Content.Versions
{
	public partial class RecentVersions : EditUserControl
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			CurrentItem = Selection.SelectedItem;
		}

		protected override void  OnDataBinding(EventArgs e)
		{
			var allVersions = Engine.Resolve<IVersionManager>().GetVersionsOf(CurrentItem.VersionOf.Value ?? CurrentItem, 4);

			ShowMoreVersions = allVersions.Count > 3
				&& Engine.SecurityManager.IsAuthorized(Page.User, CurrentItem, Security.Permission.Publish);

			Versions = allVersions.Select(v => new VersionInfo { ID = v.ID, VersionIndex = v.VersionIndex + 1, Title = v.Title, SavedBy = v.SavedBy, Info = GetVersionInfo(v), InProgress = false, State = v.State }).Take(3);

			VersionsUrl = "{ManagementUrl}/Content/Versions/".ToUrl().ResolveTokens()
				.AppendQuery(SelectionUtility.SelectedQueryKey, CurrentItem.Path)
				.AppendQuery("returnUrl", Request.RawUrl);


			var activities = Engine.Resolve<ActivityRepository<ManagementActivity>>().GetActivities(since: Utility.CurrentTime().AddHours(-1));
			Activities = activities.Where(a => a.Path == CurrentItem.Path)
				.GroupBy(a => new { a.PerformedBy, a.Operation })
				.Select(ag => ag.OrderByDescending(a => a.AddedDate).FirstOrDefault())
				.OrderByDescending(a => a.AddedDate)
				.Take(5)
				.ToList();
			ShowActivities = Activities.Count > 0;

			base.OnDataBinding(e);
		}

		private string GetVersionInfo(ContentItem v)
		{
			if (v.Expires.HasValue)
				return GetLocalResourceObject("tdExpired.Label").ToString() + v.Expires.Value;
			if (v.State == ContentState.Waiting)
				return GetLocalResourceObject("tdFuturePublish.Label").ToString() + v["FuturePublishDate"];
			if (v.State == ContentState.Draft)
				return GetLocalResourceObject("tdActivity.Label").ToString() + v["FuturePublishDate"];

			return "";
		}

		public IEnumerable<VersionInfo> Versions { get; set; }
	
		public bool ShowMoreVersions { get; set; }

		public ContentItem CurrentItem { get; set; }

		public string VersionsUrl { get; set; }

		public List<ManagementActivity> Activities { get; set; }

		public bool ShowActivities { get; set; }
	}
}