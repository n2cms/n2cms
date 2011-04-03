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
        IconUrl = "{ManagementUrl}/Resources/icons/bin.png", 
		TemplateUrl = "{ManagementUrl}/Content/Trash/Default.aspx")]
	[AllowedChildren(typeof(ContentItem))]
	[ItemAuthorizedRoles(Roles = new string[0])]
    [NotThrowable]
	public class TrashContainerItem : N2.ContentItem, ITrashCan, ISystemNode
	{
		[EditableText("Number of days to keep deleted items", 100)]
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

		public override string IconUrl
		{
			get
			{
				return Context.Current.ManagementPaths.ResolveResourceUrl(
					this.Children.Count > 0
						? "{ManagementUrl}/Resources/icons/bin.png"
						: "{ManagementUrl}/Resources/icons/bin_closed.png"
					);
			}
		}
	}
}
