using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

namespace N2.Edit
{
	/// <summary>
	/// A plugin added to the edit item toolbar area.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class EditToolbarPluginAttribute : AdministrativePluginAttribute
	{
		private string userControlUrl;

		public EditToolbarPluginAttribute(string userControlUrl)
		{
			this.userControlUrl = userControlUrl;
		}

		public override Control AddTo(Control container)
		{
			Control c = container.Page.LoadControl(userControlUrl);
			container.Controls.Add(c);
			return c;
		}
	}
}
