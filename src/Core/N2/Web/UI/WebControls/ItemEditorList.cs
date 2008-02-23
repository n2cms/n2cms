using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Definitions;

[assembly : WebResource("N2.Resources.add.gif", "image/gif")]
[assembly : WebResource("N2.Resources.bin.gif", "image/gif")]
[assembly : WebResource("N2.Resources.delete.gif", "image/gif")]

namespace N2.Web.UI.WebControls
{
	public class ItemEditorList : WebControl
	{
		#region Fields

		private DropDownList types;
		private PlaceHolder itemEditorsContainer;
		private ContentItem parentItem;
		private List<string> addedTypes = new List<string>();
		private List<ItemEditor> itemEditors = new List<ItemEditor>();
		private List<int> deletedIndexes = new List<int>();
		private int itemEditorIndex = 0;

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

		public IList<string> AddedTypes
		{
			get { return addedTypes; }
		}

		public IList<int> DeletedIndexes
		{
			get { return deletedIndexes; }
		}

		public IDefinitionManager Definitions
		{
			get { return N2.Context.Definitions; }
		}

		public ItemDefinition CurrentItemDefinition
		{
			get { return Definitions.GetDefinition(Type.GetType(ParentItemType)); }
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

			Console.WriteLine("addedTypes: " + addedTypes.Count + ", deletedIndexes: " + deletedIndexes.Count);
		}

		protected override void CreateChildControls()
		{
			foreach (ContentItem item in GetItems())
			{
				CreateItemEditor(item);
			}

			foreach (ItemDefinition definition in Definitions.GetAllowedChildren(CurrentItemDefinition, ZoneName, Page.User))
			{
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
			else
				return new ContentItem[0];
		}

		private ContentItem CreateItem(Type itemType)
		{
			ContentItem item = N2.Context.Definitions.CreateInstance(itemType, ParentItem);
			item.ZoneName = ZoneName;
			return item;
		}

		private void InitNewItemDropDown()
		{
			types = new DropDownList();
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
			AddDeleteButton();
			ItemEditor itemEditor = AddItemEditor(item);
			++itemEditorIndex;
			return itemEditor;
		}

		private void AddDeleteButton()
		{
			ImageButton b = new ImageButton();
			itemEditorsContainer.Controls.Add(b);
			b.ID = ID + "_d_" + itemEditorIndex;
			b.CssClass = " delete";
			b.ImageUrl = Page.ClientScript.GetWebResourceUrl(typeof (ItemEditorList), "N2.Resources.delete.gif");
			b.ToolTip = "Delete item";
			b.CommandArgument = itemEditorIndex.ToString();
			b.Click += DeleteItemClick;
		}

		private void DeleteItemClick(object sender, ImageClickEventArgs e)
		{
			ImageButton b = (ImageButton)sender;
			b.Enabled = false;
			b.CssClass += " disabled";

			int index = int.Parse(b.CommandArgument);
			DeletedIndexes.Add(index);
			ItemEditors[index].Enabled = false;
			ItemEditors[index].CssClass += " disabled";
		}

		private ItemEditor AddItemEditor(ContentItem item)
		{
			ItemEditor itemEditor = new ItemEditor();
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
			fs.Legend = N2.Context.Definitions.GetDefinition(item.GetType()).Title;
			container.Controls.Add(fs);
			fs.Controls.Add(itemEditor);
		}

		#region IItemContainer Members

		public ContentItem ParentItem
		{
			get
			{
				return parentItem 
					?? (parentItem = N2.Context.Persister.Get(ParentItemID));
			}
			set
			{
				if (value == null)
					throw new ArgumentNullException("value");

				parentItem = value;
				ParentItemID = value.ID;
				ParentItemType = value.GetType().AssemblyQualifiedName;
			}
		}

		#endregion
	}
}