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

namespace N2.Details
{
    /// <summary>Class applicable attribute used to mark properties/details on a content item as editable. This attribute is used in situations were base class properties or details that havn't been blessed with a property are editable. This is used to associate the web control used for the editing with the property on the content item.</summary>
	/// <example>
	/// [N2.Details.WithEditable("A certain detail", typeof(TextBox), "Text", 10, "ACertainDetail")]
	/// public abstract class AbstractBaseItem : N2.ContentItem 
	/// {
	///	}
	/// </example>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class WithEditableAttribute : EditableAttribute
    {
        /// <summary>
        /// Initializes a new instance of the WithEditableAttribute class set to use a server control.
        /// </summary>
        /// <param name="title">The label displayed to editors</param>
        /// <param name="editorType">The type of webcontrol used for editing the unit's property</param>
        /// <param name="editorPropertyName">The property on the edit control that will update the unit's property</param>
        /// <param name="sortOrder">The order of this editor</param>
        /// <param name="name">The name of property or detail on the content item to edit.</param>
        public WithEditableAttribute(string title, Type editorType, string editorPropertyName, int sortOrder, string name)
            : base(title, editorType, editorPropertyName, sortOrder)
        {
            this.Name = name;
        }
    }
}
