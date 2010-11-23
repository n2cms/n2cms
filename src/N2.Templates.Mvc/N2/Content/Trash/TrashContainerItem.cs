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
        IconUrl = "|Management|/Resources/icons/bin.png", 
		TemplateUrl = "|Management|/Content/Trash/Default.aspx")]
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
				var url = new Url(Context.Current.EditUrlManager.ResolveManagementInterfaceUrl("|Management|/Trash/Default.aspx"));

				return url.AppendQuery(PathData.PageQueryKey, ID);
			}
		}

		public override string IconUrl
		{
			get
			{
				return Context.Current.EditUrlManager.ResolveManagementInterfaceUrl(
					this.Children.Count > 0
						? "|Management|/Resources/icons/bin.png"
						: "|Management|/Resources/icons/bin_closed.png"
					);
			}
		}
	}
}
