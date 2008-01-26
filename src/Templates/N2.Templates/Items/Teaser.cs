using System;
using System.Collections.Generic;
using System.Text;
using N2.Integrity;
using N2.Details;
using System.Web.UI.WebControls;
using N2.Web.UI.WebControls;

namespace N2.Templates.Items
{
	[Item("Teaser", "Teaser")]
	[AllowedZones("RecursiveRight", "Right")]
	[WithEditableTitle("Title", 10)]
	public class Teaser : AbstractItem
	{
		[Displayable(typeof(H4), "Text")]
		public override string Title
		{
			get { return base.Title; }
			set { base.Title = value; }
		}

		[EditableTextBox("Linked text", 100, TextMode = TextBoxMode.MultiLine)]
		public virtual string LinkText
		{
			get { return (string)GetDetail("LinkText"); }
			set { SetDetail("LinkText", value, string.Empty); }
		}

		[EditableUrl("Link", 100)]
		public virtual string LinkUrl
		{
			get { return (string)GetDetail("LinkUrl"); }
			set { SetDetail("LinkUrl", value, string.Empty); }
		}

		public override string TemplateUrl
		{
			get
			{
				return "~/Parts/Teaser.ascx";
			}
		}

		public override string IconUrl
		{
			get
			{
				return "~/Img/heart.png";
			}
		}
	}
}
