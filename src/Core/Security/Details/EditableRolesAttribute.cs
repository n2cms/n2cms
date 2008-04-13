using System;
using System.Collections.Generic;
using System.Text;
using N2.Details;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace N2.Security.Details
{
	public class EditableRolesAttribute : AbstractEditableAttribute
	{
		public override bool UpdateItem(ContentItem item, Control editor)
		{
			return false;
		}

		public override void UpdateEditor(ContentItem item, Control editor)
		{
			DetailCollection dc = item.GetDetailCollection("Roles", false);
			if (dc != null)
			{
				string roles = string.Empty;
				foreach (string role in dc)
				{
					roles += role + ", ";
				}
				Literal l = (Literal)editor;
				l.Text = roles.TrimEnd(',', ' ');
			}
		}

		protected override Control AddEditor(Control container)
		{
			Literal l = new Literal();
			container.Controls.Add(l);
			return l;
		}
	}

}
