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
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Diagnostics;

namespace N2.Web
{
    /// <summary>A HttpModule that ensures that the NHibernate session is closed.</summary>
	[Obsolete("Use InitializerModule instead.")]
	public class CloseSessionModule : IHttpModule
    {
        public void Dispose()
        {
		}

        public void Init(HttpApplication context)
        {
			throw new N2Exception("The CloseSessionModule has been deprecated, replace it with the N2.Web.InitializerModule in web.config's httpModules section.");
		}
    }
}
