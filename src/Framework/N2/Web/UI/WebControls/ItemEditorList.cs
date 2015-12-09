using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Definitions;
using N2.Edit;
using N2.Engine;
using N2.Persistence;
using N2.Web.Parts;
using N2.Collections;
using System.Web.UI.HtmlControls;
using N2.Edit.Versioning;

namespace N2.Web.UI.WebControls
{
    public class ItemEditorList : Panel
    {
        #region Fields

        private readonly Engine.Logger<ItemEditorList> logger;
        private readonly Panel addPanel = new Panel { CssClass = "addArea form-actions" };
        private IDefinitionManager definitions;
        private int itemEditorIndex;
        private HtmlGenericControl itemEditorsContainer = new HtmlGenericControl("div");
        private Type minimumType = typeof (ContentItem);
        private ContentItem parentItem;
        private PartsAdapter partsAdapter;

        #endregion

        #region Constructor

        public ItemEditorList()
        {
            CssClass = "itemListEditor";
			AllowedTemplateKeys = new string[0];
			ItemEditors = new List<ItemEditor>();
			AddedDefinitions = new List<string>();
			DeletedIndexes = new List<int>();
		}

        #endregion

        #region Properties

        public string Label { get; set; }

        public List<ItemEditor> ItemEditors { get; set; }

		/// <summary>Gets the parent item where to look for items.</summary>
		public int ParentItemID
        {
            get { return (int) (ViewState["CurrentItemID"] ?? 0); }
            set { ViewState["CurrentItemID"] = value; }
        }

        /// <summary>Gets the parent item where to look for items.</summary>
        public string ParentItemType
        {
            get { return (string) (ViewState["CurrentItemType"] ?? string.Empty); }
            set { ViewState["CurrentItemType"] = value; }
        }

        /// <summary>Gets or sets the zone name to list.</summary>
        public string ZoneName
        {
            get { return (string)ViewState["ZoneName"]; }
            set { ViewState["ZoneName"] = value; }
        }

        /// <summary>The minimum type to filter children by.</summary>
        public Type MinimumType
        {
            get { return Type.GetType((string)ViewState["MinimumType"] ?? typeof(object).FullName); }
            set { ViewState["MinimumType"] = (value ?? typeof(object)).AssemblyQualifiedName; }
        }

        public IList<string> AddedDefinitions { get; set; }

		public IList<int> DeletedIndexes { get; set; }

        protected virtual IEngine Engine
        {
            get { return N2.Context.Current; }
        }

        public IDefinitionManager Definitions
        {
            get { return definitions ?? (definitions = Engine.Definitions); }
        }

        public PartsAdapter Parts
        {
            get
            {
                return partsAdapter ??
                       (partsAdapter =
                        Engine.Resolve<IContentAdapterProvider>().ResolveAdapter<PartsAdapter>(ParentItem));
            }
            set { partsAdapter = value; }
        }

        public ItemDefinition CurrentItemDefinition
        {
            get { return Definitions.GetDefinition(Type.GetType(ParentItemType)); }
        }

        public IEnumerable<LinkButton> AddButtons
        {
            get { return ItemUtility.FindInChildren<LinkButton>(addPanel).Where(lb => lb.CssClass == "addButton").ToList(); }
        }

        #endregion

        public ContentItem ParentItem
        {
            get
            {
                return parentItem
                       ?? (parentItem = Engine.Persister.Get(ParentItemID));
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                parentItem = value;
                ParentItemID = value.ID;
                ParentItemType = value.GetContentType().AssemblyQualifiedName;
            }
        }

		public string[] AllowedTemplateKeys { get; set; }

		protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Controls.Add(itemEditorsContainer);

            Controls.Add(addPanel);
        }

        protected override void LoadViewState(object savedState)
        {
            var p = (Triplet) savedState;
            base.LoadViewState(p.First);
            AddedDefinitions = (List<string>) p.Second;
            DeletedIndexes = (List<int>) p.Third;
            EnsureChildControls();
            logger.Debug("addedTypes: " + AddedDefinitions.Count + ", deletedIndexes: " + DeletedIndexes.Count);
        }

        private HtmlGenericControl CreateControl(Control parent, string tagName, string className)
        {
            var hgc = new HtmlGenericControl(tagName);
            parent.Controls.Add(hgc);
            hgc.Attributes["class"] = className;
            return hgc;
        }

        protected override void CreateChildControls()
        {
            if (!string.IsNullOrEmpty(Label))
            {
                Controls.AddAt(0, new Label { Text = Label, CssClass = "editorLabel" });
            }

            foreach (ContentItem item in GetItems())
            {
                CreateItemEditor(item);
            }
			itemEditorsContainer.Attributes["class"] = "item-editor-list-items items-count-" + itemEditorsContainer.Controls.Count;

			var allowedDefinitions = Parts.GetAllowedDefinitions(ParentItem, ZoneName, Page.User);
			allowedDefinitions = allowedDefinitions.Where(d => MinimumType.IsAssignableFrom(d.ItemType));
			allowedDefinitions = allowedDefinitions.WhereAuthorized(Engine.SecurityManager, Engine.RequestContext.User, ParentItem);
            var allowedChildren = allowedDefinitions.SelectMany(d => Parts.GetTemplates(ParentItem, d))
				.WhereAllowed(ParentItem, ZoneName, Engine.RequestContext.User, Engine.Definitions, Engine.SecurityManager)
                .ToList();
			if (AllowedTemplateKeys != null)
				allowedChildren = allowedChildren.Where(td => AllowedTemplateKeys.Contains(td.Name)).ToList();
            if (allowedChildren.Count == 0)
            {
                var alert = CreateControl(addPanel, "div", "alert");
                alert.InnerHtml = "Cannot add any parts due to zone/user/type restrictions";
            }
            else if (allowedChildren.Count == 1)
            {
                var btn = CreateButton(addPanel, allowedChildren[0]);
                btn.CssClass = "btn";
            }
            else
            {
                var btnGroup = CreateControl(addPanel, "div", "btn-group");
                var toggle = CreateControl(btnGroup, "a", "btn dropdown-toggle");
                toggle.Attributes["data-toggle"] = "dropdown";
                toggle.Attributes["href"] = "#";
                toggle.InnerHtml = "<b class='fa fa-plus-circle'></b> " + (Utility.GetLocalResourceString("Add") ?? "Add") + " <b class='caret'></b>";

                var dropdownMenu = CreateControl(btnGroup, "ul", "dropdown-menu");

				foreach (var template in allowedChildren)
                {
					var li = CreateControl(dropdownMenu, "li", "");
					CreateButton(li, template);
                }
            }

            base.CreateChildControls();
        }

        private LinkButton CreateButton(Control container, TemplateDefinition template)
        {
            var button = new LinkButton
            {
				ID = "iel" + ID + "_" + template.Definition.GetDiscriminatorWithTemplateKey().Replace('/', '_'),
				Text = string.IsNullOrEmpty(template.Definition.IconUrl)
                    ? string.Format("<b class='{0}'></b> {1}", template.Definition.IconClass, template.Definition.Title)
                    : string.Format("<img src='{0}' alt='ico'/>{1}", template.Definition.IconUrl, template.Definition.Title),
                ToolTip = template.Definition.ToolTip,
                CausesValidation = false,
                CssClass = "addButton"
            };
            var closureDefinition = template.Definition;
            button.Command += (s, a) =>
            {
				var path = EnsureDraft(ParentItem);

				UpdateItemFromTopEditor(path);

				ContentItem item = CreateItem(closureDefinition);
				item.AddTo(path.CurrentItem, ZoneName);
				Utility.UpdateSortOrder(path.CurrentItem.Children).ToList();

				var cvr = Engine.Resolve<ContentVersionRepository>();
				cvr.Save(path.CurrentPage);

				RedirectToVersionOfSelf(path.CurrentPage);
			};
            container.Controls.Add(button);
            return button;
        }

        protected override object SaveViewState()
        {
            return new Triplet(base.SaveViewState(), AddedDefinitions, DeletedIndexes);
        }

        public virtual IList<ContentItem> GetItems()
        {
            if (ParentItem != null)
            {
                var filter = new Collections.AllFilter(GetFilters());
                var items = ParentItem.Children.Where(filter).ToList();
            
                foreach (string discriminator in AddedDefinitions)
                {
					var template = Engine.Resolve<ITemplateAggregator>().GetTemplate(discriminator);
                    ContentItem item = CreateItem(template.Definition);
                    items.Add(item);
                }
                return items;
            }
            return new ContentItem[0];
        }

        private IEnumerable<ItemFilter> GetFilters()
        {
            yield return new AccessFilter(Page.User, Engine.SecurityManager);

            if (MinimumType != null)
                yield return new TypeFilter(MinimumType);

            if (!string.IsNullOrEmpty(ZoneName))
                yield return new ZoneFilter(ZoneName);
        }

        private ContentItem CreateItem(ItemDefinition definition)
        {
            ContentItem item = Engine.Resolve<ContentActivator>().CreateInstance(definition.ItemType, ParentItem, definition.TemplateKey, asProxy: true);
            item.ZoneName = ZoneName;
            return item;
        }

        protected virtual ItemEditor CreateItemEditor(ContentItem item)
        {
            var itemPanel = new Panel { CssClass = "item" };
            itemPanel.Controls.Add(new Hn { Level = 3, Text = "<span>" + Engine.Definitions.GetDefinition(item).Title + "</span>", HtmlEncode = false });
            itemEditorsContainer.Controls.Add(itemPanel);
            
            var container = new Panel { CssClass = "controls" };

            container.Controls.Add(CreateMoveButton("up", "Move item up", MoveItemUpClick));
            container.Controls.Add(CreateMoveButton("down", "Move item down", MoveItemDownClick));
            container.Controls.Add(CreateDeleteButton());

            itemPanel.Controls.Add(container);

            ItemEditor itemEditor = AddItemEditor(item, itemPanel);
            ++itemEditorIndex;
            return itemEditor;
        }

        private LinkButton CreateDeleteButton()
        {
            var b = new LinkButton();
            b.ID = ID + "_d_" + itemEditorIndex;
			b.Text = "<b class='fa fa-trash-o'></b>";
            b.ToolTip = "Delete item";
            b.CommandArgument = itemEditorIndex.ToString();
            b.CausesValidation = false;
			b.Command += DeleteItemClick;

            return b;
        }

        private LinkButton CreateMoveButton(string direction, string tooltip, CommandEventHandler handler)
        {
            var b = new LinkButton();
            b.ID = ID + "_" + direction + "_" + itemEditorIndex;
			b.Text = "<b class='fa fa-arrow-" + direction + "'></b>";
            b.ToolTip = tooltip;
            b.CommandArgument = itemEditorIndex.ToString();
            b.CausesValidation = false;
            b.Command += handler;

            return b;
        }

        private void DeleteItemClick(object sender, CommandEventArgs e)
		{
			ContentItem item = GetAssociatedItem(sender);
			var path = EnsureDraft(item);

			if (path.CurrentItem != null && path.CurrentItem != path.CurrentPage)
			{
				UpdateItemFromTopEditor(path);

				path.CurrentItem.AddTo(null);
				var cvr = Engine.Resolve<ContentVersionRepository>();
				cvr.Save(path.CurrentPage);
			}

			var url = Engine.ManagementPaths.GetEditExistingItemUrl(path.CurrentPage.FindPartVersion(parentItem), Page.Request["returnUrl"]);
			Page.Response.Redirect(url);
		}

		private void MoveItemUpClick(object sender, CommandEventArgs e)
		{
			Sort(sender, -1);
		}

		private void MoveItemDownClick(object sender, CommandEventArgs e)
        {
			Sort(sender, 1);
        }

		private void Sort(object sender, int offset)
		{
			ContentItem item = GetAssociatedItem(sender);
			var path = EnsureDraft(item);
			
            if (path.CurrentItem != null && path.CurrentItem != path.CurrentPage)
			{
				var parent = path.CurrentItem.Parent;
				var siblings = parent.Children;
				var newIndex = siblings.IndexOf(path.CurrentItem) + offset;
				if (newIndex >= 0 && newIndex < path.CurrentItem.Parent.Children.Count - 1)
				{
					Utility.Insert(path.CurrentItem, parent, newIndex);
					Utility.UpdateSortOrder(siblings).ToList();

					UpdateItemFromTopEditor(path);

					var cvr = Engine.Resolve<ContentVersionRepository>();
					cvr.Save(path.CurrentPage);
				}
			}

			RedirectToVersionOfSelf(path.CurrentPage);
		}

		private void UpdateItemFromTopEditor(PathData path)
		{
			var editor = FindTopEditor(Parent);
			var draftOfTopEditor = path.CurrentPage.FindPartVersion(editor.CurrentItem);
			editor.UpdateObject(new Edit.Workflow.CommandContext(editor.Definition, draftOfTopEditor, Interfaces.Editing, Context.User));
		}

		private ItemEditor FindTopEditor(Control parent)
		{
			var editor = ItemUtility.FindInParents<ItemEditor>(parent);
			if (editor == null)
				return null;
			return FindTopEditor(editor.Parent) ?? editor;
		}

		private ContentItem GetAssociatedItem(object sender)
		{
			var b = (LinkButton)sender;

			int index = int.Parse(b.CommandArgument);
			var item = ItemEditors[index].CurrentItem;
			return item;
		}

		private PathData EnsureDraft(ContentItem item)
		{
			var page = Find.ClosestPage(item);

			if (page.ID == 0)
				return new PathData(page, item);

			var cvr = Engine.Resolve<ContentVersionRepository>();
			var vm = Engine.Resolve<IVersionManager>();
			var path = PartsExtensions.EnsureDraft(vm, cvr, "", item.GetVersionKey(), item);

			return path;
		}

		private void RedirectToVersionOfSelf(ContentItem versionOfPage)
		{
			var url = Engine.ManagementPaths.GetEditExistingItemUrl(versionOfPage.FindPartVersion(ParentItem), Page.Request["returnUrl"]);
			Page.Response.Redirect(url);
		}

		private ItemEditor AddItemEditor(ContentItem item, Control container)
        {
            var itemEditor = new ItemEditor();
            itemEditor.ID = ID + "_ie_" + itemEditorIndex;
            container.Controls.Add(itemEditor);
            itemEditor.ZoneName = ZoneName;
            ItemEditors.Add(itemEditor);
            itemEditor.CurrentItem = item;
            return itemEditor;
        }
    }
}
