using System;
using System.Collections.Generic;
using System.Text;
using N2.Integrity;
using N2.Details;

namespace N2.Templates.Items
{
	[Item("Control Panel", "ControlPanel")]
	[AllowedZones("RecursiveRight", "Right")]
	[WithEditableTitle("Title", 10)]
	public class ControlPanel : SidebarItem
	{
		public override string TemplateUrl
		{
			get
			{
				return "~/Parts/ControlPanel.ascx";
			}
		}

		public override string IconUrl
		{
			get
			{
				return "~/Img/controller.png";
			}
		}
	}
}
