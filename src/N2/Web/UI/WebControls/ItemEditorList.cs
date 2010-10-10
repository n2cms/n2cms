using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Definitions;
using N2.Edit;
using N2.Web.Parts;
using N2.Engine;
using System.ComponentModel;
using System.Diagnostics;

[assembly: WebResource("N2.Resources.add.gif", "image/gif")]
[assembly: WebResource("N2.Resources.bin.gif", "image/gif")]
[assembly: WebResource("N2.Resources.delete.gif", "image/gif")]
[assembly: WebResource("N2.Resources.bullet_arrow_up.png", "image/png")]
[assembly: WebResource("N2.Resources.bullet_arrow_down.png", "image/png")]

namespace N2.Web.UI.WebControls
{
	public class ItemEditorList : WebControl
	{
		#region Fields

        DropDownList types = new DropDownList();
		PlaceHolder itemEditorsContainer;
		ContentItem parentItem;
		List<string> addedTypes = new List<string>();
		List<ItemEditor> itemEditors = new List<ItemEditor>();
		List<int> deletedIndexes = new List<int>();
		int itemEditorIndex = 0;
        IDefinitionManager definitions;
        PartsAdapter partsAdapter;
        Type minimumType = typeof(ContentItem);
		#endregion

		#region Constructor

		public ItemEditorList()
		{
			CssClass = "itemListEditor";
		}

		#endregion

		#region Properties

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
			get { return (string) (ViewState["ZoneName"] ?? ""); }
			set { ViewState["ZoneName"] = value; }
        }

        /// <summary>The minimum type to filter children by.</summary>
        public Type MinimumType
        {
            get { return minimumType; }
            set { minimumType = value; }
        }

		public IList<string> AddedTypes
		{
			get { return addedTypes; }
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
			get { return partsAdapter ?? (partsAdapter = Engine.Resolve<IContentAdapterProvider>().ResolveAdapter<PartsAdapter>(ParentItem.GetContentType())); }
            set { partsAdapter = value; }
        }

		public ItemDefinition CurrentItemDefinition
		{
			get { return Definitions.GetDefinition(Type.GetType(ParentItemType)); }
        }

        [NotifyParentProperty(true)]
        public DropDownList Types
        {
            get { return types; }
        }

		#endregion

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			itemEditorsContainer = new PlaceHolder();
			Controls.Add(itemEditorsContainer);

			InitNewItemDropDown();
		}

		protected override void LoadViewState(object savedState)
		{
			Triplet p = (Triplet)savedState;
			base.LoadViewState(p.First);
			addedTypes = (List<string>)p.Second;
			deletedIndexes = (List<int>)p.Third;
			EnsureChildControls();

			Debug.WriteLine("addedTypes: " + addedTypes.Count + ", deletedIndexes: " + deletedIndexes.Count);
		}

		protected override void CreateChildControls()
		{
			foreach (ContentItem item in GetItems())
			{
				CreateItemEditor(item);
			}

			foreach (ItemDefinition definition in Parts.GetAllowedDefinitions(ParentItem, ZoneName, Page.User))
			{
                if (!minimumType.IsAssignableFrom(definition.ItemType))
                {
                    continue;
                }

				ListItem li = new ListItem(definition.Title, string.Format("{0},{1}", definition.ItemType.FullName, definition.ItemType.Assembly.FullName));
				types.Items.Add(li);
			}

			base.CreateChildControls();
		}

		protected override object SaveViewState()
		{
			return new Triplet(base.SaveViewState(), addedTypes, deletedIndexes);
		}

		public virtual IList<ContentItem> GetItems()
		{
			if (ParentItem != null)
			{
				IList<ContentItem> items = ParentItem.GetChildren(ZoneName);
				foreach (string itemTypeName in AddedTypes)
				{
					ContentItem item = CreateItem(Utility.TypeFromName(itemTypeName));
					items.Add(item);
				}
				return items;
			}
			return new ContentItem[0];
		}

		private ContentItem CreateItem(Type itemType)
		{
			ContentItem item = Engine.Definitions.CreateInstance(itemType, ParentItem);
			item.ZoneName = ZoneName;
			return item;
		}

		private void InitNewItemDropDown()
		{
			Controls.Add(types);

			ImageButton b = new ImageButton();
			Controls.Add(b);
			b.ImageUrl = Page.ClientScript.GetWebResourceUrl(typeof (ItemEditorList), "N2.Resources.add.gif");
			b.ToolTip = "Add item";
			b.CausesValidation = false;
			b.Click += AddItemClick;
		}

		private void AddItemClick(object sender, ImageClickEventArgs e)
		{
			AddedTypes.Add(types.SelectedValue);

			ContentItem item = CreateItem(Utility.TypeFromName(types.SelectedValue));

			CreateItemEditor(item);
		}

		protected virtual ItemEditor CreateItemEditor(ContentItem item)
		{
			var container = new Panel {CssClass = "delete"};

			container.Controls.Add(CreateMoveUpButton());
			container.Controls.Add(CreateMoveDownButton());
			container.Controls.Add(CreateDeleteButton());

			itemEditorsContainer.Controls.Add(container);

			ItemEditor itemEditor = AddItemEditor(item);
			++itemEditorIndex;
			return itemEditor;
		}

		private ImageButton CreateDeleteButton()
		{
			var b = new ImageButton();
			b.ID = ID + "_d_" + itemEditorIndex;
			b.ImageUrl = Page.ClientScript.GetWebResourceUrl(typeof(ItemEditorList), "N2.Resources.delete.gif");
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
			b.ImageUrl = Page.ClientScript.GetWebResourceUrl(typeof(ItemEditorList), "N2.Resources.bullet_arrow_up.png");
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
			b.ImageUrl = Page.ClientScript.GetWebResourceUrl(typeof(ItemEditorList), "N2.Resources.bullet_arrow_down.png");
			b.ToolTip = "Move item down";
			b.CommandArgument = itemEditorIndex.ToString();
			b.CausesValidation = false;
			b.Click += MoveItemDownClick;

			return b;
		}

		private void DeleteItemClick(object sender, ImageClickEventArgs e)
		{
			var b = (ImageButton)sender;
			b.Enabled = false;
			b.CssClass += " disabled";

			int index = int.Parse(b.CommandArgument);
			DeletedIndexes.Add(index);
			ItemEditors[index].Enabled = false;
			ItemEditors[index].CssClass += " disabled";
			foreach(var validator in N2.Web.UI.ItemUtility.FindInChildren<IValidator>(ItemEditors[index]))
			{
				if (validator is BaseValidator)
					(validator as BaseValidator).Enabled = false;
				if(Page.Validators.Contains(validator))
					Page.Validators.Remove(validator);
			}
		}

		private void MoveItemUpClick(object sender, ImageClickEventArgs e)
		{
			var b = (ImageButton)sender;

			int index = int.Parse(b.CommandArgument);
			var item = ItemEditors[index].CurrentItem;

			var itemIndex = item.Parent.Children.IndexOf(item) - 1;

			if(itemIndex < 0)
				return;

            Engine.Resolve<ITreeSorter>().MoveTo(item, NodePosition.Before, item.Parent.Children[itemIndex]);

			Context.Response.Redirect(Context.Request.Url.PathAndQuery);
		}

		private void MoveItemDownClick(object sender, ImageClickEventArgs e)
		{
			var b = (ImageButton)sender;

			int index = int.Parse(b.CommandArgument);
			var item = ItemEditors[index].CurrentItem;

			var itemIndex = item.Parent.Children.IndexOf(item) + 1;

			if (itemIndex == item.Parent.Children.Count)
				return;

            Engine.Resolve<ITreeSorter>().MoveTo(item, NodePosition.After, item.Parent.Children[itemIndex]);

			Context.Response.Redirect(Context.Request.Url.PathAndQuery);
		}

		private ItemEditor AddItemEditor(ContentItem item)
		{
			var itemEditor = new ItemEditor();
			itemEditor.ID = ID + "_ie_" + itemEditorIndex;
			AddToContainer(itemEditorsContainer, itemEditor, item);
			itemEditor.ZoneName = ZoneName;
			itemEditors.Add(itemEditor);
			itemEditor.CurrentItem = item;
			return itemEditor;
		}

		protected virtual void AddToContainer(Control container, ItemEditor itemEditor, ContentItem item)
		{
			FieldSet fs = new FieldSet();
            fs.Legend = Engine.Definitions.GetDefinition(item.GetContentType()).Title;
			container.Controls.Add(fs);
			fs.Controls.Add(itemEditor);
		}

		#region IItemContainer Members

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

		#endregion
	}
}
