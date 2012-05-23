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
using System.Web.UI.WebControls;
using N2.Persistence.Search;
using System.IO;
using N2.Web.UI.WebControls;

namespace N2.Details
{
	/// <summary>
	/// Attribute used to mark properties as editable. This attribute is predefined to use 
	/// the <see cref="System.Web.UI.WebControls.TextBox"/> web control as editor.</summary>
	/// <example>
	/// [N2.Details.EditableText("Heading", 80)]
	/// public virtual string Heading { get; set; }
	/// 
	/// [N2.Details.EditableText("Published", 80)]
	/// public virtual DateTime PublishedDate { get; set; }
	/// </example>
	[AttributeUsage(AttributeTargets.Property)]
	public class EditableTextAttribute : AbstractEditableAttribute, IDisplayable, IWritingDisplayable, IIndexableProperty
	{
		private int maxLength = 0;
		private int columns = 0;
		private int rows = 0;
		private TextBoxMode textMode = TextBoxMode.SingleLine;

		public EditableTextAttribute()
			: base(null, 50)
		{
			IsIndexable = true;
		}

		/// <summary>Initializes a new instance of the EditableTextBoxAttribute class.</summary>
		/// <param name="title">The label displayed to editors</param>
		/// <param name="sortOrder">The order of this editor</param>
		public EditableTextAttribute(string title, int sortOrder)
			: base(title, sortOrder)
		{
			IsIndexable = true;
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

	    #endregion

		#region IDisplayable Members

		Control IDisplayable.AddTo(ContentItem item, string detailName, Control container)
		{
			using (var sw = new StringWriter())
			{
				Write(item, detailName, sw);

				string text = sw.ToString();
				if (string.IsNullOrEmpty(text))
					return null;

				Literal l = new Literal();
				l.Text = text;
				container.Controls.Add(l);
				return l;

			}
		}

		#endregion

		public override bool UpdateItem(ContentItem item, Control editor)
		{
			TextBox tb = editor as TextBox;
			string value = tb.Text;
			if (DefaultValue is string && tb.Text == (string)DefaultValue)
				value = null;
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
			tb.Text = Utility.Convert<string>(item[Name]) ?? DefaultValue as string;
		}

		/// <summary>Creates a text box editor.</summary>
		/// <param name="container">The container control the tetx box will be placed in.</param>
		/// <returns>A text box control.</returns>
		protected override Control AddEditor(Control container)
		{
			TextBox tb = CreateEditor();
            tb.Placeholder(GetLocalizedText("Placeholder") ?? Placeholder);
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
			tb.TextMode = TextMode;
		}

		#region IWritingDisplayable Members

		public virtual void Write(ContentItem item, string propertyName, System.IO.TextWriter writer)
		{
			writer.Write(item[propertyName]);
		}

		#endregion

		#region IIndexableProperty Members

		public bool IsIndexable { get; set; }

		public virtual string GetIndexableText(ContentItem item)
		{
			object value = item[Name];
			if (value == null)
				return null;

			return value.ToString();
		}

		#endregion
	}

	
	/// <summary>
	/// Attribute used to mark properties as editable. This attribute is predefined to use 
	/// the <see cref="System.Web.UI.WebControls.TextBox"/> web control as editor.</summary>
	/// <example>
	/// [N2.Details.EditableText("Heading", 80)]
	/// public virtual string Heading { get; set; }
	/// 
	/// [N2.Details.EditableText("Published", 80)]
	/// public override DateTime Published { get; set; }
	/// </example>
	/// <remarks>Prefer [EditableText] over this since [EditableTextBox] will be made obsolete in future releases.</remarks>
	[AttributeUsage(AttributeTargets.Property)]
	public class EditableTextBoxAttribute : EditableTextAttribute
	{
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
			: base(title, sortOrder)
		{
			MaxLength = maxLength;
		}
	}
}
