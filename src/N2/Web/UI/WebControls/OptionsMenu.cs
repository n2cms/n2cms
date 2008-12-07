using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using N2.Resources;

namespace N2.Web.UI.WebControls
{
	/// <summary>
	/// This control is used in the edit interface and needs additional styling if
	/// it's to be used somewhere else.
	/// </summary>
	public class OptionsMenu : Control
	{
		protected override void OnPreRender(System.EventArgs e)
		{
            string script = string.Format("jQuery('#{0}').n2optionmenu({{opener:\"<span class='opener'><img src='{1}' alt='more options'/></span>\"}});", ClientID, N2.Web.Url.ToAbsolute("~/Edit/img/bullet_arrow_down.gif"));
			Register.JavaScript(Page, script, ScriptOptions.DocumentReady);
			base.OnPreRender(e);
		}

		protected override void Render(HtmlTextWriter writer)
		{
			writer.Write("<div id='" + ClientID + "' class='optionGroup'>");
			RenderChildren(writer);
			writer.Write("</div>");
		}
	}
}
