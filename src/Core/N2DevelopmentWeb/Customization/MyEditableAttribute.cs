using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace N2DevelopmentWeb.Customization
{
	public class MyEditableAttribute : N2.Details.EditableAttribute
	{
		public MyEditableAttribute()
		{
			this.Title = "My Editable Attribute";
			this.SortOrder = 1000;
		}

		protected override Control CreateEditor(Control container)
		{
			TextBox tb = new TextBox();
			tb.TextMode = TextBoxMode.MultiLine;
			return tb;
		}

		protected override object GetEditorValue(Control editor)
		{
			return ((TextBox)editor).Text;
		}

		protected override void SetEditorValue(Control editor, object value)
		{
			((TextBox)editor).Text = (string)value;
		}
	}
}
