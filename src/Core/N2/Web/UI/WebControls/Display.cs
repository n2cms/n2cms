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
using System.ComponentModel;

namespace N2.Web.UI.WebControls
{
	/// <summary>
	/// Control used to display content on a web forms page. The control will 
	/// look for an attribute implementing IDisplayable on the property 
	/// referenced by the PropertyName on the display control.
	/// </summary>
	/// <example>
	/// <!-- Displays the page title using the default displayable. -->
	/// &lt;n2:Display PropertyName="Title" runat="server" /&gt;
	/// </example>
	[PersistChildren(false)]
	[ParseChildren(true)]
	public class Display : Control, IItemContainer
	{
		private IDisplayable displayable = null;
		private Control displayer;
		ITemplate headerTemplate;
		ITemplate footerTemplate;

		/// <summary>Gets the displayable attribute</summary>
		public IDisplayable Displayable
		{
			get { return displayable ?? (displayable = GetDisplayableAttribute(PropertyName, CurrentItem)); }
		}

		/// <summary>Gets the control responsible of displaying the detail.</summary>
		public Control Displayer
		{
			get { return displayer; }
		}

		/// <summary>Use the displayer and the values from this path.</summary>
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

		/// <summary>Prevent this control from throwing exceptions when irregularities are discovered, e.g. there is no property with the given name on the page.</summary>
		public bool SwallowExceptions
		{
			get { return (bool)(ViewState["SwallowExceptions"] ?? false); }
			set { ViewState["SwallowExceptions"] = value; }
		}

		/// <summary>Inserted before the display control if a control was added.</summary>
		[DefaultValue((string)null), Browsable(false), PersistenceMode(PersistenceMode.InnerProperty), TemplateContainer(typeof(DisplayTemplateContainer))]
		public virtual ITemplate HeaderTemplate
		{
			get { return this.headerTemplate; }
			set { this.headerTemplate = value; }
		}

		/// <summary>Added after the display control if a control was added.</summary>
		[DefaultValue((string)null), Browsable(false), PersistenceMode(PersistenceMode.InnerProperty), TemplateContainer(typeof(DisplayTemplateContainer))]
		public virtual ITemplate FooterTemplate
		{
			get { return this.footerTemplate; }
			set { this.footerTemplate = value; }
		}
 



		protected override void OnInit(EventArgs e)
		{
			AddDisplayable();
			base.OnInit(e);
		}

		protected void AddDisplayable()
		{
			if (Displayable != null)
			{
				displayer = Displayable.AddTo(CurrentItem, PropertyName, this);

				if (displayer != null)
				{
					if (HeaderTemplate != null)
					{
						Control header = new DisplayTemplateContainer();
						this.Controls.AddAt(0, header);

						HeaderTemplate.InstantiateIn(header);
					}

					if (FooterTemplate != null)
					{
						Control footer = new DisplayTemplateContainer();
						this.Controls.Add(footer);

						FooterTemplate.InstantiateIn(footer);
					}
				}
			}
		}


		private IDisplayable GetDisplayableAttribute(string propertyName, ContentItem item)
		{
			if (item == null)
			{
				return Throw<IDisplayable>(new ArgumentNullException("item"));
			}

			PropertyInfo pi = item.GetType().GetProperty(propertyName);
			if (pi == null)
			{
				return Throw<IDisplayable>(new N2Exception("No property {0} found the item of type {1}", propertyName, item.GetType()));
			}
			else
			{
				IDisplayable[] attributes = (IDisplayable[])pi.GetCustomAttributes(typeof(IDisplayable), false);
				if (attributes.Length == 0)
				{
					return Throw<IDisplayable>(new N2Exception("No attribute implementing IDisplayable found on the property {0} of the type {1}",
										  propertyName, item.GetType()));
				}
				return attributes[0];
			}
		}

		private T Throw<T>(Exception ex)
			where T : class
		{
			if (SwallowExceptions)
				return null;
			throw ex;
		}

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