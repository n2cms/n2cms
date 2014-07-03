using N2.Definitions;
using N2.Details;
using N2.Management.Api;
using N2.Installation;
using N2.Integrity;

namespace N2.Edit.Trash
{
    [PageDefinition("Trash",
        Name = "TrashContainerItem",
        InstallerVisibility = InstallerHint.NeverRootOrStartPage,
        IconClass = "fa fa-trash-o",
        TemplateUrl = "{ManagementUrl}/Content/Trash/Default.aspx",
        AuthorizedRoles = new string[0])]
    [AllowedChildren(typeof(ContentItem))]
    [Throwable(AllowInTrash.No)]
    [InterfaceFlags("Management", "Unclosable")]
    public class TrashContainerItem : N2.ContentItem, ITrashCan, ISystemNode
    {
        [EditableNumber("Number of days to keep deleted items", 100)]
        public virtual int KeepDays
        {
            get { return (int)(GetDetail("KeepDays") ?? 31); }
            set { SetDetail("KeepDays", value); }
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

        [EditableCheckBox("Empty trash asynchrnously", 100)]
        public virtual bool AsyncTrashPurging
        {
            get { return GetDetail("AsyncTrashPurging", true); }
            set { SetDetail("AsyncTrashPurging", value, true); }
        }

        public override string IconClass
        {
            get
            {
                return base.IconClass + (this.Children.Count > 0 ? string.Empty : " opaque");
            }
        }
    }
}
