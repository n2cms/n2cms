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
using System.Web.UI.WebControls;
using System.Web.UI;

namespace N2.Details
{
	/// <summary>
	/// Attribute used to mark properties as editable. This attribute is predefined to use 
	/// the <see cref="System.Web.UI.WebControls.TextBox"/> web control as editor.</summary>
	/// <example>
	/// [N2.Details.EditableTextBox("Heading", 80)]
	/// public virtual string Heading
	/// {
	/// 	get { return GetDetail("Heading", "")); }
	/// 	set { SetDetail("Heading", value, ""); }
	/// }
	/// 
	/// [N2.Details.EditableTextBox("Published", 80)]
	/// public override DateTime Published
	/// {
    ///     get { return base.Published; } 
    ///     set { base.Published = value; }
	/// }
	/// </example>
	[AttributeUsage(AttributeTargets.Property)]
	public class EditableTextBoxAttribute : AbstractEditableAttribute, IDisplayable
	{
		private int maxLength = 0;
		private int columns = 0;
		private int rows = 0;
		private TextBoxMode textMode = TextBoxMode.SingleLine;
		private string defaultValue = string.Empty;

		public EditableTextBoxAttribute()
			: base(null, 50)
		{
		}

		/// <summary>Initializes a new instance of the EditableTextBoxAttribute class.</summary>
		/// <param name="title">The label displayed to editors</param>
		/// <param name="sortOrder">The order of this editor</param>
		public EditableTextBoxAttribute(string title, int sortOrder)
			: base(title, sortOrder)
		{
		}

		/// <summary>Initializes a new instance of the EditableTextBoxAttribute class.</summary>
		/// <param name="title">The label displayed to editors</param>
		/// <param name="sortOrder">The order of this editor</param>
		/// <param name="maxLength">The max length of the text box.</param>
		public EditableTextBoxAttribute(string title, int sortOrder, int maxLength)
			: this(title, sortOrder)
		{
			this.maxLength = maxLength;
		}

		#region Properties

		/// <summary>Gets or sets columns on the text box.</summary>
		public int Columns
		{
			get { return columns; }
			set { columns = value; }
		}

		/// <summary>Gets or sets rows on the text box.</summary>
		public int Rows
		{
			get { return rows; }
			set { rows = value; }
		}

		/// <summary>Gets or sets the text box mode.</summary>
		public TextBoxMode TextMode
		{
			get { return textMode; }
			set { textMode = value; }
		}

		/// <summary>Gets or sets the max length of the text box.</summary>
		public int MaxLength
		{
			get { return maxLength; }
			set { maxLength = value; }
		}

		/// <summary>Gets or sets the default value. When the editor's value equals this value then null is saved instead.</summary>
		public string DefaultValue
		{
			get { return defaultValue; }
			set { defaultValue = value; }
		}

	    #endregion

		#region IDisplayable Members

		Control IDisplayable.AddTo(ContentItem item, string detailName, Control container)
		{
			string text = item[detailName] as string;
		
			if(string.IsNullOrEmpty(text))
				return null;

			Literal l = new Literal();
			l.Text = text;
			container.Controls.Add(l);
			return l;
		}

		#endregion

		public override bool UpdateItem(ContentItem item, Control editor)
		{
			TextBox tb = editor as TextBox;
			string value = (tb.Text == DefaultValue) ? null : tb.Text;
			if(!AreEqual(value, item[Name]))
			{
				item[Name] = value;
				return true;
			}
			return false;
		}

		public override void UpdateEditor(ContentItem item, Control editor)
		{
			TextBox tb = editor as TextBox;
			tb.Text = Utility.Convert<string>(item[Name]) ?? DefaultValue;
		}

		/// <summary>Creates a text box editor.</summary>
		/// <param name="container">The container control the tetx box will be placed in.</param>
		/// <returns>A text box control.</returns>
		protected override Control AddEditor(Control container)
		{
			TextBox tb = CreateEditor();
            tb.ID = Name;
            tb.CssClass += " textEditor";
			ModifyEditor(tb);
			container.Controls.Add(tb);

			return tb;
		}

        /// <summary>Instantiates the text box control.</summary>
        /// <returns>A text box.</returns>
		protected virtual TextBox CreateEditor()
		{
			return new TextBox();
		}

		protected virtual void ModifyEditor(TextBox tb)
		{
			if (MaxLength > 0) tb.MaxLength = MaxLength;
			if (Columns > 0) tb.Columns = Columns;
			if (Rows > 0) tb.Rows = Rows;
			if (Columns > 0) tb.Rows = Rows;
			tb.TextMode = TextMode;
		}
	}
}
