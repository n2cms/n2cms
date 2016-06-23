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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Definitions;
using N2.Edit;
using N2.Edit.Web;
using N2.Edit.Workflow;
using N2.Engine;
using N2.Persistence;
using N2.Resources;
using N2.Edit.Versioning;
using System.Linq;

namespace N2.Web.UI.WebControls
{
    /// <summary>A form that generates an edit interface for content items.</summary>
    public class ItemEditor : WebControl, INamingContainer, IItemEditor, IContentForm<CommandContext>, IPlaceHolderAccessor
    {
		protected override void LoadViewState(object savedState)
		{
			base.LoadViewState(savedState);
		}

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
        private IDictionary<string, Control> placeholders = new Dictionary<string, Control>();
        #endregion

        #region Properties

        public Type ContainerTypeFilter { get; set; }

        public string[] EditableNameFilter { get; set; }

        public virtual IEngine Engine { get; set; }

        /// <summary>The content adapter related to the current page item.</summary>
        protected virtual EditableAdapter EditAdapter
        {
            get { return Engine.Resolve<IContentAdapterProvider>().ResolveAdapter<EditableAdapter>(CurrentItem); }
        }

        /// <summary>Gets a dictionary of editor controls added this control.</summary>
        public IDictionary<string, Control> AddedEditors { get; protected set; }

        protected override HtmlTextWriterTag TagKey
        {
            get { return HtmlTextWriterTag.Div; }
        }

        /// <summary>The type of item to edit. ItemEditor will look at <see cref="N2.Details.AbstractEditableAttribute"/> attributes on this type to render input controls.</summary>
        public string Discriminator
        {
            get { return (string)ViewState["Discriminator"] ?? string.Empty; }
            set { ViewState["Discriminator"] = value; }
        }

        /// <summary>The definition which is the source of editors.</summary>
        public ItemDefinition Definition { get; set; }

        /// <summary>Gets the type of the edited item.</summary>
        /// <returns>The item's type.</returns>
        public Type CurrentItemType
        {
            get
            {
                var definition = GetDefinition();
                if (definition != null)
                    return definition.ItemType;

                return null;
            }
        }

        /// <summary>Gets the parent item id that the edited item will be set to.</summary>
        public string ParentPath
        {
            get { return (string)(ViewState["ParentPath"] ?? string.Empty); }
            set { ViewState["ParentPath"] = value; }
        }

        /// <summary>Gets or sets the item to edit with this form.</summary>
        public ContentItem CurrentItem
        {
            get
            {
                if (currentItem == null && !string.IsNullOrEmpty(Discriminator))
                {
                    ContentItem parentItem = Engine.Resolve<Navigator>().Navigate(HttpUtility.UrlDecode(ParentPath));
                    if (ParentVersionIndex.HasValue)
                    {
                        parentItem = Engine.Resolve<IVersionManager>().GetVersion(parentItem, ParentVersionIndex.Value);
                    }
                    if (!string.IsNullOrEmpty(ParentVersionKey))
                    {
                        parentItem = parentItem.FindDescendantByVersionKey(ParentVersionKey);
                    }
                    currentItem = Engine.Resolve<ContentActivator>().CreateInstance(CurrentItemType, parentItem, null, asProxy: true);
                    currentItem.ZoneName = ZoneName;
                }
                return currentItem;
            }
            set
            {
                var previous = currentItem;
                currentItem = value;
                if (value != null)
                {
                    Definition = Engine.Definitions.GetDefinition(value);
                    Discriminator = Definition.Discriminator;
                    if (value.VersionOf.HasValue && value.ID == 0)
                        VersioningMode = ItemEditorVersioningMode.SaveOnly;

                    if (previous != null && previous != currentItem)
                        Controls.Clear();

                    EnsureChildControls();
                }
                else
                {
                    Discriminator = null;
                }
            }
        }

        /// <summary>Gets or sets the zone name that the edited item will be set to.</summary>
        public string ZoneName
        {
            get { return (string)ViewState["ZoneName"]; }
            set { ViewState["ZoneName"] = value; }
        }

        /// <summary>Gets or sets wether a version should be saved before the item is updated.</summary>
        public ItemEditorVersioningMode VersioningMode
        {
            get { return (ItemEditorVersioningMode)(ViewState["SaveVersion"] ?? ItemEditorVersioningMode.VersionAndSave); }
            set { ViewState["SaveVersion"] = value; }
        }

		public bool EnableAutoSave { get; set; }

        #endregion

        #region Methods

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            Register.JavaScript(Page, @"
    $('.helpPanel').click(function () {
        var $hp = $(this);
        $hp.toggleClass('helpVisible');
    });

    // hide mce toolbar to prevent it getting skewed
    $('.tabs a').click(function () {
        $('.mceExternalToolbar').hide();
    });
    $('input').focus(function () {
        $('.mceExternalToolbar').hide();
    });

    $('.dimmable').n2dimmable();

    $('.uploader > label').n2revealer();

    $('.expandable').n2expandable({ visible: '.uncontractable' });

    $('form').n2expandableBox({ opener: '.rightOpener', opened: '#outside' });
    $('#outside .box').n2expandableBox({ opener: 'h4', opened: '.box-inner' });
", ScriptOptions.DocumentReady);

            Register.StyleSheet(Page, Url.ResolveTokens("{ManagementUrl}/Resources/Css/edit.css"));

			Page.ClientScript.RegisterHiddenField(ClientID + "_autosaved_item_id", Page.Request[ClientID + "_autosaved_item_id"] ?? currentItem.ID.ToString());
			if (EnableAutoSave)
			{
				Register.JavaScript(Page, @"	window.n2autosave && n2autosave.init();", ScriptOptions.DocumentReady);

				TryAddItemReference(this);
				foreach (var placeholder in placeholders.Values)
				{
					if (!TryAddItemReference(placeholder as WebControl))
						if (placeholder.Controls.Count > 0)
							TryAddItemReference(placeholder.Controls[0] as WebControl);
				}
			}
			else
			{
				TryAddDisabledAutosave(this);
				foreach (var placeholder in placeholders.Values)
				{
					if (!TryAddDisabledAutosave(placeholder as WebControl))
						if (placeholder.Controls.Count > 0)
							TryAddDisabledAutosave(placeholder.Controls[0] as WebControl);
				}
			}
        }

		private bool TryAddDisabledAutosave(WebControl control)
		{
			if (control == null)
				return false;
			TryAddItemReference(control);
			control.Attributes["data-disable-autosave"] = "true";
			return true;
		}

		private bool TryAddItemReference(WebControl control)
		{
			if (control == null)
				return false;
			control.Attributes["data-item"] = CurrentItem.ID.ToString();
			control.Attributes["data-item-version-key"] = CurrentItem.GetVersionKey();
			control.Attributes["data-page"] = CurrentItem.IsPage
				? currentItem.ID.ToString()
				: Find.ClosestPage(CurrentItem) != null
					? Find.ClosestPage(CurrentItem).ID.ToString()
					: "";
			control.Attributes["data-item-zone"] = CurrentItem.ZoneName;
			control.Attributes["data-item-reference"] = ClientID + "_autosaved_item_id";
			return true;
		}

        protected override void CreateChildControls()
        {
            var definition = GetDefinition();

            if (definition != null)
            {
                AddedEditors = EditAdapter.AddDefinedEditors(definition, CurrentItem, this, Page.User, ContainerTypeFilter, EditableNameFilter);
                if (!Page.IsPostBack)
                {
                    EditAdapter.LoadAddedEditors(definition, CurrentItem, AddedEditors, Page.User);
                }
            }

            base.CreateChildControls();
        }

        public ItemDefinition GetDefinition()
        {
            return Definition
                ?? Engine.Definitions.GetDefinition(Discriminator);
        }

        /// <summary>Saves <see cref="CurrentItem"/> with the values entered in the form.</summary>
        [Obsolete("Use CommandDispatcher")]
        public ContentItem Save(ContentItem item, ItemEditorVersioningMode mode)
        {
            EnsureChildControls();
            BinderContext = CreateCommandContext();
            item = EditAdapter.SaveItem(item, AddedEditors, mode, Page.User);
            if (Saved != null)
                Saved.Invoke(this, new ItemEventArgs(item));
            return item;
        }

        /// <summary>Saves <see cref="CurrentItem"/> with the values entered in the form.</summary>
        /// <returns>The saved item.</returns>
        [Obsolete("Use CommandDispatcher")]
        public ContentItem Save()
        {
            CurrentItem = Save(CurrentItem, VersioningMode);
            return CurrentItem;
        }

        /// <summary>Updates the <see cref="CurrentItem"/> with the values entered in the form without saving it.</summary>
        public void Update()
        {
            UpdateObject(CreateCommandContext());
        }

        public void Reload()
        {
            ChildControlsCreated = false;
            CreateChildControls();
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
		public event Action<object, CommandContext> CreatingContext;

		#endregion

		#region IBinder<CommandContext> Members

		public N2.Edit.Workflow.CommandContext BinderContext { get; internal set; }

        public bool UpdateObject(N2.Edit.Workflow.CommandContext value)
        {
            try
            {
                BinderContext = value;
                EnsureChildControls();
                if (ZoneName != null && ZoneName != value.Content.ZoneName)
                    value.Content.ZoneName = ZoneName;
                foreach (string key in AddedEditors.Keys)
                    BinderContext.GetDefinedDetails().Add(key);
                var modifiedDetails = EditAdapter.UpdateItem(value.Definition, value.Content, AddedEditors, Page.User);
                if (modifiedDetails.Length == 0)
                    return false;
                foreach (string detailName in modifiedDetails)
                    BinderContext.GetUpdatedDetails().Add(detailName);
                BinderContext.RegisterItemToSave(value.Content);
                return true;
            }
            finally
            {
                BinderContext = null;
            }
        }

        public void UpdateInterface(N2.Edit.Workflow.CommandContext value)
        {
            try
            {
                BinderContext = value;
                EnsureChildControls();
                Engine.EditManager.UpdateEditors(GetDefinition(), value.Content, AddedEditors, Page.User);
            }
            finally
            {
                BinderContext = null;
            }

        }

        #endregion

        #region IPlaceHolderAccessor Members

        public void AddPlaceHolder(string name, Control container)
        {
            placeholders[name] = container;
        }

        public Control GetPlaceHolder(string name)
        {
            Control container;
            placeholders.TryGetValue(name, out container);
            return container;
        }

        #endregion

        public CommandContext CreateCommandContext()
        {
            var cc = new CommandContext(Definition ?? GetDefinition(), CurrentItem, Interfaces.Editing, Page.User, this, new PageValidator<CommandContext>(Page));

			TryReplaceContentWithAutosavedVersion(cc);

			if (CreatingContext != null)
				CreatingContext(this, cc);

            return cc;
        }

		private int[] GetAutosavedIdAndVersion()
		{
			var autoSaveReference = Page.Request[ClientID + "_autosaved_item_id"];
			if (Page.IsPostBack && !string.IsNullOrEmpty(autoSaveReference))
				return autoSaveReference.Split('.').Select(x => int.Parse(x)).ToArray();
			return new int[0];
        }

		public ContentItem GetAutosaveVersion()
		{
			var idAndVersion = GetAutosavedIdAndVersion();
			if (idAndVersion == null && idAndVersion.Length < 1)
				return null;
			if (idAndVersion[0] == 0)
				return null;
			var item = Engine.Persister.Get(idAndVersion[0]);
			if (item != null && idAndVersion.Length > 1 && idAndVersion[1] != 0)
				item = Engine.Resolve<IVersionManager>().GetVersion(item, idAndVersion[1]);
			return item;
		}

		private bool TryReplaceContentWithAutosavedVersion(CommandContext cc)
		{
			var item = GetAutosaveVersion();
			if (item == null)
				return false;
			if (cc.Content.ID == 0)
				cc.Content.AddTo(null);
			cc.Content = item;
			return true;
		}

        public void Initialize(string discriminator, string template, ContentItem parent)
        {
            var definition = Engine.Definitions.GetDefinition(discriminator);

            if (!string.IsNullOrEmpty(template))
            {
                var info = Engine.Resolve<ITemplateAggregator>().GetTemplate(definition.ItemType, template);
                if (info == null)
                    throw new InvalidOperationException("Failed to find definition for type " + definition.ItemType + " and template " + template);
                Definition = info.Definition;
                var item = info.TemplateFactory();
                item.Parent = parent;
                CurrentItem = item;
            }
            else
            {
                Discriminator = definition.Discriminator;
                ParentPath = parent.Path;
                if (parent.ID == 0)
                {
                    ParentPath = Find.ClosestPage(parent).Path;
                    ParentVersionIndex = parent.VersionIndex;
                    ParentVersionKey = parent.GetVersionKey();
                }
                CurrentItem = Engine.Resolve<ContentActivator>().CreateInstance(CurrentItemType, parent, null, asProxy: true);
            }
            EnsureChildControls();
        }

        public void Clear()
        {
            ClearChildState();
            ChildControlsCreated = false;
        }

        public int? ParentVersionIndex
        {
            get { return (int?)ViewState["ParentVersionIndex"]; }
            set { ViewState["ParentVersionIndex"] = value; }
        }

        public string ParentVersionKey
        {
            get { return (string)ViewState["ParentVersionKey"]; }
            set { ViewState["ParentVersionKey"] = value; }
        }
    }
}
