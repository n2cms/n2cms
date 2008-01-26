#region License

/* Copyright (C) 2007 Cristian Libardo
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 */

#endregion

using System;
using System.Reflection;
using System.Web.UI;
using N2.Details;

namespace N2.Web.UI.WebControls
{
	/// <summary>Container control for displayable items.</summary>
	public class Display : Control, IItemContainer
	{
		#region Private Members

		private IDisplayable displayable = null;
		private Control displayer;

		#endregion

		#region Properties

		/// <summary>Gets the displayable attribute</summary>
		public IDisplayable Displayable
		{
			get
			{
				if (displayable == null)
					displayable = GetDisplayableAttribute(PropertyName, CurrentItem);
				return displayable;
			}
		}

		/// <summary>Gets the control responsible of displaying the detail.</summary>
		public Control Displayer
		{
			get { return displayer; }
		}

		public string Path
		{
			get { return (string) (ViewState["Path"] ?? string.Empty); }
			set { ViewState["Path"] = value; }
		}

		/// <summary>The name of the property on the content item whose value is displayed with the Display control.</summary>
		public string PropertyName
		{
			get { return (string) ViewState["PropertyName"] ?? ""; }
			set { ViewState["PropertyName"] = value; }
		}

		#endregion

		#region Methods

		private static IDisplayable GetDisplayableAttribute(string propertyName, ContentItem item)
		{
			if (item == null)
				throw new ArgumentNullException("item");

			PropertyInfo pi = item.GetType().GetProperty(propertyName);
			if (pi == null)
			{
				throw new N2Exception("No property {0} found the item of type {1}", propertyName, item.GetType());
			}
			else
			{
				IDisplayable[] attributes = (IDisplayable[]) pi.GetCustomAttributes(typeof (IDisplayable), false);
				if (attributes.Length == 0)
				{
					throw new N2Exception("No attribute implementing IDisplayable found on the property {0} of the type {1}",
					                      propertyName, item.GetType());
				}
				return attributes[0];
			}
		}

		protected override void OnInit(EventArgs e)
		{
			AddDisplayable();
			base.OnInit(e);
		}

		protected override void CreateChildControls()
		{
			base.CreateChildControls();
		}

		protected void AddDisplayable()
		{
			IDisplayable displayable = GetDisplayableAttribute(PropertyName, CurrentItem);
			if (displayable != null)
			{
				displayer = displayable.AddTo(CurrentItem, PropertyName, this);
			}
		}

		#endregion

		#region IItemContainer Members

		private ContentItem currentItem = null;

		public ContentItem CurrentItem
		{
			get
			{
				if (currentItem == null)
				{
					currentItem = ItemUtility.FindCurrentItem(Parent);
					if (!string.IsNullOrEmpty(Path))
						currentItem = ItemUtility.WalkPath(currentItem, Path);
				}
				return currentItem;
			}
		}

		#endregion
	}
}