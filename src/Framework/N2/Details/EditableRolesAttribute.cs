using System.Collections.Generic;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Details;
using System;

namespace N2.Security.Details
{
	[AttributeUsage(AttributeTargets.Property)]
	public class EditableRolesAttribute : AbstractEditableAttribute
	{
		public override bool UpdateItem(ContentItem item, Control editor)
		{
			CheckBoxList cbl = editor as CheckBoxList;
			List<string> roles = new List<string>();
			foreach (ListItem li in cbl.Items)
				if (li.Selected)
					roles.Add(li.Value);

			DetailCollection dc = item.GetDetailCollection(Name, true);
			dc.Replace(roles);

			return true;
		}

		public override void UpdateEditor(ContentItem item, Control editor)
		{
			CheckBoxList cbl = editor as CheckBoxList;
			DetailCollection dc = item.GetDetailCollection(Name, false);
			if (dc != null)
			{
				foreach (string role in dc)
				{
					ListItem li = cbl.Items.FindByValue(role);
					if (li != null)
					{
						li.Selected = true;
					}
					else
					{
						li = new ListItem(role);
						li.Selected = true;
						li.Attributes["style"] = "color:silver";
						cbl.Items.Add(li);
					}
				}
			}
		}

		protected override Control AddEditor(Control container)
		{
			CheckBoxList cbl = new CheckBoxList();
			foreach (string role in Roles.GetAllRoles())
			{
				cbl.Items.Add(role);
			}
			container.Controls.Add(cbl);
			return cbl;
		}
	}

}
