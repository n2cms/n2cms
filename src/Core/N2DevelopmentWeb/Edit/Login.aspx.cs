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
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace N2.Edit
{
    public partial class Login : Web.EditPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
			this.Login1.Focus();
        }

		protected void Login1_LoggingIn(object sender, LoginCancelEventArgs e)
		{
		}

		protected void Login1_LoginError(object sender, EventArgs e)
		{
		}

		protected void Login1_Authenticate(object sender, AuthenticateEventArgs e)
		{
			try
			{
				if (FormsAuthentication.Authenticate(Login1.UserName, Login1.Password))
				{
					e.Authenticated = true;
					FormsAuthentication.RedirectFromLoginPage(Login1.UserName, Login1.RememberMeSet);
				}
				else if (Membership.ValidateUser(Login1.UserName, Login1.Password))
				{
					e.Authenticated = true;
					FormsAuthentication.RedirectFromLoginPage(Login1.UserName, Login1.RememberMeSet);
				}
			}
			catch (Exception ex)
			{
				Trace.Warn(ex.ToString());
				e.Authenticated = false;
			}
		}
    }
}
