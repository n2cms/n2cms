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

using System;
using System.Web.UI;

namespace N2.Web.UI
{
    /// <summary>
    /// Provides data about a control. These event arguments can be used to 
    /// expose child controls while they are added.
    /// </summary>
    public class ControlEventArgs : EventArgs
    {
        /// <summary>Creates a new instance of the ControlEventArgs.</summary>
        /// <param name="control">The control to reference with these arguments.</param>
        public ControlEventArgs(Control control)
        {
            this.control = control;
        }

        private Control control;
        /// <summary>The control associated with these arguments.</summary>
        public Control Control
        {
            get { return control; }
        }
    }
}
