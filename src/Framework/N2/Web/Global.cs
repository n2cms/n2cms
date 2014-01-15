#region License
/* Copyright (C) 2007 Cristian Libardo
 *
 * This is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as
 * published by the Free Software Foundation; either version 2.1 of
 * the License, or (at your option) any later version.
 *
 * This software is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this software; if not, write to the Free
 * Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
 * 02110-1301 USA, or see the FSF site: http://www.fsf.org.
 */
#endregion

using N2.Engine;

namespace N2.Web
{
    /// <summary>
    /// Enables N2 to interact with the ASP.NET application request to do url 
    /// rewriting, authorizing requests and closing NHibernate sessions.
    /// </summary>
    public class Global : System.Web.HttpApplication
    {
        /// <summary>Initializes the N2 factory.</summary>
        public override void Init()
        {
            base.Init();

            EventBroker.Instance.Attach(this);

            N2.Context.Initialize(false);
        }
    }
}
