#region License
/* Copyright (C) 2006 Cristian Libardo
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA.
 */
#endregion

using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;

namespace N2.Edit
{
    public partial class Default : Web.EditPage
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			try
			{
				// These fields are used client side to store selected items
				Page.ClientScript.RegisterHiddenField("selected", SelectedItem.Path);
				Page.ClientScript.RegisterHiddenField("memory", "");
				Page.ClientScript.RegisterHiddenField("action", "");
			}
			catch(NullReferenceException ex)
			{
				string url = GetSelectedPath();
				if(url == null)
					throw  new N2Exception("Couldn't get the start page, this usually indicates a configuration or installation problem. The start page must be inserted and it's id must be configured in web.config.", ex);
				else 
					Response.Write("Error: Couldn't find '" + url + "'.");
			}
		}
	}
}