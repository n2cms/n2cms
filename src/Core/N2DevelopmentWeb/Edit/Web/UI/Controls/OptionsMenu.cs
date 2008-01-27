#region License
/* Copyright (C) 2007 Cristian Libardo
 *
 * This is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as
 * published by the Free Software Foundation; either version 2.1 of
 * the License, or (at your option) any later version.
 */
#endregion

using System.Web.UI;
using N2.Resources;

namespace N2.Edit.Web.UI.Controls
{
	public class OptionsMenu : Control
	{
		protected override void OnPreRender(System.EventArgs e)
		{
            string script = string.Format("$('#{0}').n2optionmenu({{opener:\"<span class='opener'><img src='{1}' alt='more options'/></span>\"}});", ClientID, Utility.ToAbsolute("~/Edit/img/ico/bullet_arrow_down.gif"));
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
