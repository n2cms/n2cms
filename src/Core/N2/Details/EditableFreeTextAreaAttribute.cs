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
using N2.Web.UI.WebControls;

namespace N2.Details
{
	/// <summary>Attribute used to mark properties as editable. This attribute is predefined to use the <see cref="N2.Web.UI.WebControls.FreeTextArea"/> web control as editor.</summary>
	/// <example>
	/// [N2.Details.EditableFreeTextArea("Text", 110)]
    /// public virtual string Text
    /// {
	///     get { return (string)GetDetail("Text"); }
    ///		set { SetDetail("Text", value); }
	/// }
	/// </example>
	[AttributeUsage(AttributeTargets.Property)]
	public class EditableFreeTextAreaAttribute : EditableTextBoxAttribute
	{
		public EditableFreeTextAreaAttribute(string title, int sortOrder) 
			: base(title, sortOrder)
		{
		}

		protected override void ModifyEditor(TextBox tb)
		{
			// do nothing
		}

		protected override TextBox CreateEditor()
		{
			return new FreeTextArea();
		}

		protected override Control AddRequiredFieldValidator(Control container, Control editor)
		{
			RequiredFieldValidator rfv = base.AddRequiredFieldValidator(container, editor) as RequiredFieldValidator;
			rfv.EnableClientScript = false;
			return rfv;
		}
	}
}
