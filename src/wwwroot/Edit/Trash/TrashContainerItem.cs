using System.Web;
using N2.Integrity;
using N2.Definitions;
using N2.Details;
using N2.Installation;

namespace N2.Edit.Trash
{
	[PageDefinition("Trash", 
		Name = "TrashContainerItem", 
		InstallerVisibility = InstallerHint.NeverRootOrStartPage,
		IconUrl = "~/Edit/Trash/Img/bin.gif")]
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


		public override string TemplateUrl
		{
			get
			{
				return "~/Edit/Trash/Default.aspx";
			}
		}

		public override string IconUrl
		{
			get
			{
				return VirtualPathUtility.ToAbsolute(
					this.Children.Count > 0 ?
						"~/Edit/Trash/Img/bin.gif" :
						"~/Edit/Trash/Img/bin_closed.gif"
					);
			}
		}
	}
}
