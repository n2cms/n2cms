﻿using System;
using N2.Details;
using N2.Web.UI;

namespace N2.Management.Myself
{
	[PartDefinition("Activity", 
		TemplateUrl = "|Management|/Myself/Activity.ascx",
		IconUrl = "|Management|/Resources/icons/text_list_numbers.png")]
	[WithEditableTitle("Title", 10)]
	public class ActivityPart : RootPartBase
	{
		public override string Title
		{
			get { return base.Title ?? "Activity"; }
			set { base.Title = value; }
		}

		[EditableTextBox("Latest changes max count", 100)]
		public virtual int LatestChangesMaxCount
		{
			get { return (int)(GetDetail("LatestChangesMaxCount") ?? 5); }
			set { SetDetail("LatestChangesMaxCount", value); }
		}
	}

	public partial class Activity : ContentUserControl<ContentItem, ActivityPart>
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			rptLatestChanges.DataSource = Find.Items.All.MaxResults(CurrentItem.LatestChangesMaxCount).OrderBy.Updated.Desc.Select();
			DataBind();
		}
	}
}