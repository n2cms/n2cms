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
using System.Web.UI.WebControls;
using System.Web.UI;

namespace N2.Details
{
    /// <summary>Class applicable attribute used to add a title editor.</summary>
    /// <example>
    /// [N2.Details.WithEditableTitle("Menu text", 10)]
    /// public abstract class AbstractBaseItem : N2.ContentItem 
    /// {
    ///	}
    /// </example>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class WithEditableTitleAttribute : AbstractEditableAttribute
    {
		private bool focus = true;

		/// <summary>
		/// Creates a new instance of the WithEditableAttribute class with default values.
		/// </summary>
		public WithEditableTitleAttribute()
			: this("Title", 0)
		{
		}

		/// <summary>
		/// Creates a new instance of the WithEditableAttribute class with default values.
		/// </summary>
		/// <param name="title">The label displayed to editors</param>
		/// <param name="sortOrder">The order of this editor</param>
		public WithEditableTitleAttribute(string title, int sortOrder)
			: base(title, "Title", sortOrder)
		{
			Required = true;
		}

		/// <summary>Gets or sets whether the title editor should receive focus.</summary>
		public bool Focus
		{
			get { return focus; }
			set { focus = value; }
		}

		public override bool UpdateItem(ContentItem item, Control editor)
		{
			TextBox tb = (TextBox)editor;
			if (item.Title != tb.Text)
			{
				item.Title = tb.Text;
				return true;
			}
			return false;
		}

		public override void UpdateEditor(ContentItem item, Control editor)
		{
			TextBox titleTextBox = (TextBox)editor;
			titleTextBox.Text = item.Title;

			if (Focus)
				titleTextBox.PreRender += FocusSender;
		}

    	static void FocusSender(object sender, EventArgs e)
		{
			TextBox titleTextBox = (TextBox)sender;
			titleTextBox.Focus();
		}

		protected override Control AddEditor(Control container)
		{
			TextBox tb = new TextBox();
			tb.ID = Name;
			tb.MaxLength = 250;
			tb.CssClass = "titleEditor";
			container.Controls.Add(tb);
			return tb;
		}
    }
}
