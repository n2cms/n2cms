using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using N2;

namespace N2DevelopmentWeb.Domain
{
	[N2.Definition("Item With Links", "MyItemWithLinks")]
	[N2.Integrity.RestrictParents(typeof(MyPageData), typeof(MyItemWithLinks))]
	public class MyItemWithLinks : AbstractCustomItem
	{

		[N2.Details.Editable("Int Value", typeof(System.Web.UI.WebControls.TextBox), "Text", 100)]
		public virtual int IntValue
		{
			get { return (int)(GetDetail("IntValue") ?? 0); }
			set { SetDetail("IntValue", value); }
		}


		[N2.Details.Editable("Referenced item", typeof(N2.Web.UI.WebControls.ItemSelector), "SelectedItem", 100)]
		public virtual ContentItem ReferencedItem
		{
			get { return (ContentItem)GetDetail("ReferencedItem"); }
			set { SetDetail("ReferencedItem", value); }
		}

		[N2.Details.Editable("Referenced item 2", typeof(N2.Web.UI.WebControls.ItemSelector), "SelectedItem", 100)]
		public virtual ContentItem ReferencedItem2
		{
			get { return (ContentItem)GetDetail("ReferencedItem2"); }
			set { SetDetail("ReferencedItem2", value); }
		}

		public override string TemplateUrl
		{
			get
			{
				return "~/Templates/LinkedDetail.aspx";
			}
		}
	}
}
