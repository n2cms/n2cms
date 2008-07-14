#region License

/* Copyright (C) 2006-2007 Cristian Libardo
 *
 * This is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as
 * published by the Free Software Foundation; either version 2.1 of
 * the License, or (at your option) any later version.
 */

#endregion

using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Engine;
using N2.Persistence;
using N2.Definitions;

namespace N2.Web.UI.WebControls
{
	/// <summary>A form that generates an edit interface for content items.</summary>
	public class ItemEditor : WebControl, INamingContainer, IItemEditor
	{
		#region Constructor

		public ItemEditor()
		{
			CssClass = "itemEditor";
			Engine = N2.Context.Current;
		}

		public ItemEditor(ContentItem item)
			: this()
		{
			CurrentItem = item;
		}

		#endregion

		#region Private Fields

		private ContentItem currentItem;
		private IDictionary<string, Control> editors;
		private IEngine engine;

		#endregion

		#region Properties

		public virtual IEngine Engine
		{
            get { return engine ?? N2.Context.Current; }
			set { engine = value; }
		}

		/// <summary>Gets a dictionary of editor controls added this control.</summary>
		public IDictionary<string, Control> AddedEditors
		{
			get { return editors; }
		}

		protected override HtmlTextWriterTag TagKey
		{
			get { return HtmlTextWriterTag.Div; }
		}

        ///// <summary>The type of item to edit. ItemEditor will look at <see cref="N2.EditableAttribute"/> attributes on this type to render input controls.</summary>
        //public string ItemTypeName
        //{
        //    get { return (string) ViewState["ContentItemType"] ?? ""; }
        //    set { ViewState["ContentItemType"] = value; }
        //}

        /// <summary>The type of item to edit. ItemEditor will look at <see cref="N2.EditableAttribute"/> attributes on this type to render input controls.</summary>
        public string Discriminator
        {
            get { return (string)ViewState["Discriminator"] ?? string.Empty; }
            set { ViewState["Discriminator"] = value; }
        }

        /// <summary>Gets the parent item id that the edited item will be set to.</summary>
        public string ParentPath
        {
            get { return (string)(ViewState["ParentPath"] ?? string.Empty); }
            set { ViewState["ParentPath"] = value; }
        }

        //[Obsolete]
        ///// <summary>Gets the parent item id that the edited item will be set to.</summary>
        //public int ParentItemID
        //{
        //    get { return (int) (ViewState["ParentItemID"] ?? 0); }
        //    set { ViewState["ParentItemID"] = value; }
        //}

		/// <summary>Gets or sets the item to edit with this form.</summary>
		public ContentItem CurrentItem
		{
			get
			{
				if (currentItem == null && !string.IsNullOrEmpty(Discriminator))
				{
                    ContentItem parentItem = Engine.Resolve<N2.Edit.Navigator>().Navigate(ParentPath);//.Persister.Get(ParentItemID);
					currentItem = Engine.Definitions.CreateInstance(CurrentItemType, parentItem);
					currentItem.ZoneName = ZoneName;
				}
				return currentItem;
			}
			set
			{
				currentItem = value;
				if (value != null)
				{
					//ItemTypeName = value.GetType().AssemblyQualifiedName;
                    Discriminator = Engine.Definitions.GetDefinition(value.GetType()).Discriminator;
					if (value.VersionOf != null && value.ID == 0)
						VersioningMode = ItemEditorVersioningMode.SaveOnly;
					EnsureChildControls();
					Engine.EditManager.UpdateEditors(value, AddedEditors, Page.User);
				}
				else
				{
                    Discriminator = null;//ItemTypeName = null;
				}
			}
		}

		/// <summary>Gets or sets the zone name that the edited item will be set to.</summary>
		public string ZoneName
		{
			get { return (string) (ViewState["ZoneName"] ?? ""); }
			set { ViewState["ZoneName"] = value; }
		}

		/// <summary>Gets or sets wether a version should be saved before the item is updated.</summary>
		public ItemEditorVersioningMode VersioningMode
		{
			get { return (ItemEditorVersioningMode) (ViewState["SaveVersion"] ?? ItemEditorVersioningMode.VersionAndSave); }
			set { ViewState["SaveVersion"] = value; }
		}

        /// <summary>Gets the type defined by <see cref="ItemTypeName"/>.</summary>
        /// <returns>The item's type.</returns>
        public Type CurrentItemType
        {
            get 
            {
                if (!string.IsNullOrEmpty(Discriminator))
                {
                    ItemDefinition def = Engine.Definitions.GetDefinition(Discriminator);
                    if (def != null)
                    {
                        return def.ItemType;
                    }
                }
                return null;
            }
        }

		#endregion

		#region Methods

        //public bool HasType()
        //{
        //    return CurrentItemType != null;
        //}

		protected override void CreateChildControls()
		{
            Type t = CurrentItemType;
			if (t != null)
			{
				editors = Engine.EditManager.AddEditors(t, this, Page.User);
				if (!Page.IsPostBack)
				{
					Engine.EditManager.UpdateEditors(CurrentItem, AddedEditors, Page.User);
				}
			}

			base.CreateChildControls();
		}

		/// <summary>Saves <see cref="CurrentItem"/> with the values entered in the form.</summary>
		public ContentItem Save()
		{
			EnsureChildControls();
			CurrentItem = Engine.EditManager.Save(this, Page.User);
			if (Saved != null)
				Saved.Invoke(this, new ItemEventArgs(CurrentItem));
			return CurrentItem;
		}

		/// <summary>Updates the <see cref="CurrentItem"/> with the values entered in the form without saving it.</summary>
		public void Update()
		{
			Engine.EditManager.UpdateItem(CurrentItem, AddedEditors, Page.User);
		}

		#endregion

		#region IItemContainer Members

		ContentItem IItemContainer.CurrentItem
		{
			get { return CurrentItem; }
		}

		#endregion

		#region IItemEditor Members

		public event EventHandler<ItemEventArgs> Saved;

		#endregion
	}
}