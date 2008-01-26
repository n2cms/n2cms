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
using System.Reflection;
using N2.Definitions;

namespace N2.Web.UI
{
	/// <summary>Attribute used to modify a detail's editor.</summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = true)]	
    public class EditorModifierAttribute : Attribute, IUniquelyNamed
    {
		#region Constructor
		public EditorModifierAttribute(string editorPropertyName, object value)
		{
			this.editorPropertyName = editorPropertyName;
			this.value = value;
		} 
		#endregion

		#region Private Members
		private string name;
		private string editorPropertyName;
		private object value; 
		#endregion

		#region Public Properties
		/// <summary>Gets or sets the name of the detail whose editor should have this modifier applied. When the attribute is specified on a property in the content item class that property is used.</summary>
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		/// <summary>Gets or sets the name of the property on the editor to set.</summary>
		public string EditorPropertyName
		{
			get { return editorPropertyName; }
			set { editorPropertyName = value; }
		}

		/// <summary>Gets or sets the value to apply to property with the name <see cref="EditorPropertyName"/> on the editor.</summary>
		public object Value
		{
			get { return this.value; }
			set { this.value = value; }
		} 
		#endregion

		#region Methods
		/// <summary>Applies the modifications specified by this attribute. Updates the property specified by <see cref="EditorPropertyName"/> with the value specified by <see cref="Value"/>.</summary>
		/// <param name="editorContainer">The editor control to modify.</param>
		public virtual void Modify(Control editor)
		{
			PropertyInfo pi = editor.GetType().GetProperty(EditorPropertyName);
			System.Diagnostics.Debug.Assert(pi != null, "No property with the given name found on the editor, property name: " + EditorPropertyName);
			if (pi != null)
			{
				object o = Value;
				if (o.GetType() != pi.PropertyType)
					o = Utility.Convert(o, pi.PropertyType);
				if (o != null)
					pi.SetValue(editor, o, new object[0]);
			}
		}
		#endregion
	}
}
