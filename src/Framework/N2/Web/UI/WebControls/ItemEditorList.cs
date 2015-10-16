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

namespace N2.Web.UI.WebControls
{
    public class ItemEditorList : Panel
    {
        #region Fields

        private readonly Engine.Logger<ItemEditorList> logger;
        private readonly List<ItemEditor> itemEditors = new List<ItemEditor>();
        private readonly Panel addPanel = new Panel { CssClass = "addArea form-actions" };
        private List<string> addedDefinitions = new List<string>();
        private IDefinitionManager definitions;
        private List<int> deletedIndexes = new List<int>();
        private int itemEditorIndex;
        private PlaceHolder itemEditorsContainer = new PlaceHolder();
        private Type minimumType = typeof (ContentItem);
        private ContentItem parentItem;
        private PartsAdapter partsAdapter;

        #endregion

        #region Constructor

        public ItemEditorList()
        {
            CssClass = "itemListEditor";
        }

        #endregion

        #region Properties

        public string Label { get; set; }

        public List<ItemEditor> ItemEditors
        {
            get { return itemEditors; }
        }

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

        public IList<string> AddedDefinitions
        {
            get { return addedDefinitions; }
        }

        public IList<int> DeletedIndexes
        {
            get { return deletedIndexes; }
        }

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
            addedDefinitions = (List<string>) p.Second;
            deletedIndexes = (List<int>) p.Third;
            EnsureChildControls();
            logger.Debug("addedTypes: " + addedDefinitions.Count + ", deletedIndexes: " + deletedIndexes.Count);
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

            var allowedChildren = Parts.GetAllowedDefinitions(ParentItem, ZoneName, Page.User)
                .Where(d => MinimumType.IsAssignableFrom(d.ItemType))
				.WhereAuthorized(Engine.SecurityManager, Engine.RequestContext.User, ParentItem)
				.SelectMany(d => Parts.GetTemplates(ParentItem, d))
				.WhereAllowed(ParentItem, ZoneName, Engine.RequestContext.User, Engine.Definitions, Engine.SecurityManager)
                .ToList();
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
                ContentItem item = CreateItem(closureDefinition);
                item.ZoneName = ZoneName;
                AddedDefinitions.Add(closureDefinition.GetDiscriminatorWithTemplateKey());
                CreateItemEditor(item);
            };
            container.Controls.Add(button);
            return button;
        }

        protected override object SaveViewState()
        {
            return new Triplet(base.SaveViewState(), addedDefinitions, deletedIndexes);
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
            
            var container = new Panel { CssClass = "delete" };

            container.Controls.Add(CreateMoveUpButton());
            container.Controls.Add(CreateMoveDownButton());
            container.Controls.Add(CreateDeleteButton());

            itemPanel.Controls.Add(container);

            ItemEditor itemEditor = AddItemEditor(item, itemPanel);
            ++itemEditorIndex;
            return itemEditor;
        }

        private ImageButton CreateDeleteButton()
        {
            var b = new ImageButton();
            b.ID = ID + "_d_" + itemEditorIndex;
            b.ImageUrl = Engine.ManagementPaths.ResolveResourceUrl("{ManagementUrl}/Resources/icons/delete.png");
            b.ToolTip = "Delete item";
            b.CommandArgument = itemEditorIndex.ToString();
            b.CausesValidation = false;
            b.Click += DeleteItemClick;

            return b;
        }

        private ImageButton CreateMoveUpButton()
        {
            var b = new ImageButton();
            b.ID = ID + "_up_" + itemEditorIndex;
            b.ImageUrl = Engine.ManagementPaths.ResolveResourceUrl("{ManagementUrl}/Resources/icons/bullet_arrow_up.png");
            b.ToolTip = "Move item up";
            b.CommandArgument = itemEditorIndex.ToString();
            b.CausesValidation = false;
            b.Click += MoveItemUpClick;

            return b;
        }

        private ImageButton CreateMoveDownButton()
        {
            var b = new ImageButton();
            b.ID = ID + "_down_" + itemEditorIndex;
            b.ImageUrl = Engine.ManagementPaths.ResolveResourceUrl("{ManagementUrl}/Resources/icons/bullet_arrow_down.png");
            b.ToolTip = "Move item down";
            b.CommandArgument = itemEditorIndex.ToString();
            b.CausesValidation = false;
            b.Click += MoveItemDownClick;

            return b;
        }

        private void DeleteItemClick(object sender, ImageClickEventArgs e)
        {
            var b = (ImageButton) sender;
            b.Enabled = false;
            b.CssClass += " disabled";

            int index = int.Parse(b.CommandArgument);
            DeletedIndexes.Add(index);
            ItemEditors[index].Enabled = false;
            ItemEditors[index].CssClass += " disabled";
            foreach (IValidator validator in ItemUtility.FindInChildren<IValidator>(ItemEditors[index]))
            {
                if (validator is BaseValidator)
                    (validator as BaseValidator).Enabled = false;
                if (Page.Validators.Contains(validator))
                    Page.Validators.Remove(validator);
            }
        }

        private void MoveItemUpClick(object sender, ImageClickEventArgs e)
        {
            var b = (ImageButton) sender;

            int index = int.Parse(b.CommandArgument);
            ContentItem item = ItemEditors[index].CurrentItem;

            int itemIndex = item.Parent.Children.IndexOf(item) - 1;

            if (itemIndex < 0)
                return;

            Engine.Resolve<ITreeSorter>().MoveTo(item, NodePosition.Before, item.Parent.Children[itemIndex]);

            Context.Response.Redirect(Context.Request.Url.PathAndQuery);
        }

        private void MoveItemDownClick(object sender, ImageClickEventArgs e)
        {
            var b = (ImageButton) sender;

            int index = int.Parse(b.CommandArgument);
            ContentItem item = ItemEditors[index].CurrentItem;

            int itemIndex = item.Parent.Children.IndexOf(item) + 1;

            if (itemIndex == item.Parent.Children.Count)
                return;

            Engine.Resolve<ITreeSorter>().MoveTo(item, NodePosition.After, item.Parent.Children[itemIndex]);

            Context.Response.Redirect(Context.Request.Url.PathAndQuery);
        }

        private ItemEditor AddItemEditor(ContentItem item, Control container)
        {
            var itemEditor = new ItemEditor();
            itemEditor.ID = ID + "_ie_" + itemEditorIndex;
            container.Controls.Add(itemEditor);
            itemEditor.ZoneName = ZoneName;
            itemEditors.Add(itemEditor);
            itemEditor.CurrentItem = item;
            return itemEditor;
        }
    }
}
