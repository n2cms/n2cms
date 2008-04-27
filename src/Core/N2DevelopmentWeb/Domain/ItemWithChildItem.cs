using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace N2DevelopmentWeb.Domain
{
	[N2.Definition("Item With Child Item", "ItemWithChildItem")]
	[N2.Integrity.AllowedChildren(typeof(MyItemData))]
	[N2.Web.UI.FieldSet("child", "Child item", 0)]
	[N2.Integrity.AvailableZone("Right", "Right")]
	[N2.Details.WithEditableChild(typeof(MyItemData), "MyChildItem", 10, DefaultChildZoneName = "Right", ContainerName = "child")]
	public class ItemWithChildItem : AbstractCustomItem
	{
		[N2.Details.EditableItem]
		public virtual MyItemData ChildItem
		{
			get { return (MyItemData)(GetDetail("ChildItem")); }
			set { SetDetail("ChildItem", value); }
		}

		public override string TemplateUrl
		{
			get
			{
				return "/itemwithchild.aspx";
			}
		}
	}
}
