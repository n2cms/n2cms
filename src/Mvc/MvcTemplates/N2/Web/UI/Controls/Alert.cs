using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace N2.Edit.Web.UI.Controls
{
	public enum AlertType
	{
		
	}
    public class Alert : HtmlContainerControl
    {
		private LiteralControl closeButton;
		private LiteralControl textLiteral;

		public Alert()
			: base("div")
		{
			Closable = true;
			closeButton = new LiteralControl("<button type=\"button\" class=\"close\" data-dismiss=\"alert\">&times;</button>");
			textLiteral = new LiteralControl();
		}

		public bool Closable { get; set; }

		public string AlertTypeClass { get; set; }

		public string Text { get; set; }

		protected override void CreateChildControls()
		{
			base.CreateChildControls();

			Controls.Add(closeButton);
			Controls.Add(textLiteral);
		}

		protected override void OnPreRender(EventArgs e)
		{
			closeButton.Visible = Closable;

			if (string.IsNullOrEmpty(AlertTypeClass))
				Attributes["class"] = "alert";
			else
				Attributes["class"] = "alert " + AlertTypeClass;

			textLiteral.Text = Text;

			base.OnPreRender(e);
		}
    }
}
