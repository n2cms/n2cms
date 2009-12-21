using System.Web;
using N2.Integrity;
using N2.Definitions;
using N2.Details;
using N2.Installation;
using N2.Web;

namespace N2.Edit.Trash
{
	[PageDefinition("Trash", 
		Name = "TrashContainerItem", 
		InstallerVisibility = InstallerHint.NeverRootOrStartPage,
        IconUrl = "~/Edit/img/ico/png/bin.png", 
		TemplateUrl = "~/Edit/Trash/Default.aspx")]
	[AllowedChildren(typeof(ContentItem))]
	[ItemAuthorizedRoles(Roles = new string[0])]
    [NotThrowable]
	public class TrashContainerItem : N2.ContentItem, ITrashCan
	{
		[EditableTextBox("Number of days to keep deleted items", 100)]
		public virtual int KeepDays
		{
			get { return (int)(GetDetail("KeepDays") ?? 31); }
			set { SetDetail<int>("KeepDays", value); }
		}

		[EditableCheckBox("Enabled", 80)]
		public virtual bool Enabled
		{
			get { return (bool)(GetDetail("Enabled") ?? true); }
			set { SetDetail("Enabled", value); }
		}

		[EditableEnum("Purge Interval", 90, typeof(TrashPurgeInterval))]
		public virtual TrashPurgeInterval PurgeInterval
		{
			get { return GetDetail("PurgeInterval", TrashPurgeInterval.Monthly); }
			set { SetDetail("PurgeInterval", value, TrashPurgeInterval.Monthly); }
		}

		public override string Url
		{
			get
			{
			    var url = new Url(Context.Current.EditManager.GetEditInterfaceUrl());
				
			    return url.AppendSegment("Trash/default.aspx").AppendQuery( PathData.PageQueryKey, ID);
			}
		}

		public override string IconUrl
		{
			get
			{
				return VirtualPathUtility.ToAbsolute(
					this.Children.Count > 0 ?
                        "~/Edit/img/ico/png/bin.png" :
                        "~/Edit/img/ico/png/bin_closed.png"
					);
			}
		}
	}
}
