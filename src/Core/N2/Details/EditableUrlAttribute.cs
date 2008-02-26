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
using N2.Web.UI.WebControls;

namespace N2.Details
{
	/// <summary>Attribute used to mark properties as editable. This attribute is predefined to use the <see cref="N2.Web.UI.WebControls.UrlSelector"/> web control as editor/url selector.</summary>
	/// <example>
	/// [N2.Details.EditableUrl("Url to page or document", 50)]
	/// public virtual string PageOrDocumentUrl
	/// {
	///     get { return (string)GetDetail("PageOrDocumentUrl"); }
	///		set { SetDetail("PageOrDocumentUrl", value); }
	/// }
	/// </example>
	[AttributeUsage(AttributeTargets.Property)]
	public class EditableUrlAttribute : AbstractEditableAttribute
	{
		private UrlSelectorMode openingMode = UrlSelectorMode.Items;
		private UrlSelectorMode availableModes = UrlSelectorMode.All;

		public UrlSelectorMode AvailableModes
		{
			get { return availableModes; }
			set { availableModes = value; }
		}

		public UrlSelectorMode OpeningMode
		{
			get { return openingMode; }
			set { openingMode = value; }
		}

		/// <summary>Initializes a new instance of the EditableUrlAttribute class.</summary>
		/// <param name="title">The label displayed to editors</param>
		/// <param name="sortOrder">The order of this editor</param>
		public EditableUrlAttribute(string title, int sortOrder)
			: base(title, sortOrder)
		{
		}

		public override bool UpdateItem(ContentItem item, System.Web.UI.Control editor)
		{
			UrlSelector selector = (UrlSelector)editor;
			if(selector.Url != (string)item[Name])
			{
				item[Name] = selector.Url;
				return true;
			}
			return false;
		}

		public override void UpdateEditor(ContentItem item, System.Web.UI.Control editor)
		{
			UrlSelector selector = (UrlSelector)editor;
			selector.Url = (string)item[Name];
		}

		protected override Control AddEditor(Control container)
		{
			UrlSelector selector = new UrlSelector();
			selector.ID = this.Name;
			selector.AvailableModes = AvailableModes;
			selector.DefaultMode = OpeningMode;

			container.Controls.Add(selector);

			return selector;
		}
	}
}
