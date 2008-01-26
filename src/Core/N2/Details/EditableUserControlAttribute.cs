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
using System.Web.UI;
using System.Web.UI.WebControls;

namespace N2.Details
{
    /// <summary>Attribute used to mark properties as editable. This is used to associate the control used for the editing with the property/detail on the content item whose value we are editing.</summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class EditableUserControlAttribute : EditableAttribute
    {
		#region Constructors
		/// <summary>Initializes a new instance of the EditableAttribute class set to use a user control.</summary>
		/// <param name="title">The label displayed to editors</param>
		/// <param name="userControlPath">The virtual path of a user control used for editing</param>
		/// <param name="editorPropertyName">The property on the edit control that will update the unit's property</param>
		/// <param name="sortOrder">The order of this editor</param>
		public EditableUserControlAttribute(string title, string userControlPath, string editorPropertyName, int sortOrder)
			: base(title, typeof(System.Web.UI.UserControl), editorPropertyName, sortOrder)
		{
			this.UserControlPath = userControlPath;
		}
		#endregion

		#region Private Members
		private string userControlPath;
		#endregion

		#region Properties
		/// <summary>Gets or sets the virtual path of a user control. This property is only considered when ControlType is <see cref="System.Web.UI.UserControl"/>.</summary>
		public string UserControlPath
		{
			get { return userControlPath; }
			set { userControlPath = value; }
		} 
		#endregion

		#region Methods
		protected override Control CreateEditor(Control container)
		{
			return container.Page.LoadControl(this.UserControlPath);
		}
		#endregion
	}
}
