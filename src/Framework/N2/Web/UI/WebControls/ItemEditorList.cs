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
using log4net;

namespace N2.Web.UI.WebControls
{
	public class ItemEditorList : Panel
	{
		#region Fields

		private readonly ILog logger = LogManager.GetLogger(typeof(ItemEditorList));
		private readonly List<ItemEditor> itemEditors = new List<ItemEditor>();
		private readonly Panel addPanel = new Panel { CssClass = "addArea" };
		private List<string> addedDefinitions = new List<string>();
		private IDefinitionManager definitions;
		private List<int> deletedIndexes = new List<int>();
		private int itemEditorIndex;
		private PlaceHolder itemEditorsContainer;
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
			get { return minimumType; }
			set { minimumType = value; }
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
			get { return addPanel.Controls.OfType<LinkButton>(); }
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

			itemEditorsContainer = new PlaceHolder();
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

		protected override void CreateChildControls()
		{
			foreach (ContentItem item in GetItems())
			{
				CreateItemEditor(item);
			}

			if (!string.IsNullOrEmpty(Label))
			{
				addPanel.Controls.Add(new Label { Text = Label, CssClass = "editorLabel" });
			}

			foreach (ItemDefinition definition in Parts.GetAllowedDefinitions(ParentItem, ZoneName, Page.User))
			{
				if (!minimumType.IsAssignableFrom(definition.ItemType))
				{
					continue;
				}

				var button = new LinkButton
				{
					ID = "iel" + ID + "_" + definition.GetDiscriminatorWithTemplateKey().Replace('/', '_'),
					Text = string.Format("<img src='{0}' alt='ico'/>{1}", definition.IconUrl, definition.Title),
					ToolTip = definition.ToolTip,
					CausesValidation = false
				};
				var closureDefinition = definition;
				button.Command += (s, a) =>
					{
						ContentItem item = CreateItem(closureDefinition);
						item.ZoneName = ZoneName;
						AddedDefinitions.Add(closureDefinition.GetDiscriminatorWithTemplateKey());
						CreateItemEditor(item);
					};
				addPanel.Controls.Add(button);
			}

			base.CreateChildControls();
		}

		protected override object SaveViewState()
		{
			return new Triplet(base.SaveViewState(), addedDefinitions, deletedIndexes);
		}

		public virtual IList<ContentItem> GetItems()
		{
			if (ParentItem != null)
			{
				IList<ContentItem> items = string.IsNullOrEmpty(ZoneName)
					? ParentItem.GetChildren()
					: ParentItem.GetChildren(ZoneName);
				foreach (string discriminator in AddedDefinitions)
				{
					ContentItem item = CreateItem(Definitions.GetDefinition(discriminator));
					items.Add(item);
				}
				return items;
			}
			return new ContentItem[0];
		}

		private ContentItem CreateItem(ItemDefinition definition)
		{
			ContentItem item = Engine.Resolve<ContentActivator>().CreateInstance(definition.ItemType, ParentItem, definition.TemplateKey);
			item.ZoneName = ZoneName;
			return item;
		}

		protected virtual ItemEditor CreateItemEditor(ContentItem item)
		{
			var itemPanel = new Panel { CssClass = "item" };
			itemPanel.Controls.Add(new Hn { Level = 3, Text = "<span>" + Engine.Definitions.GetDefinition(item).Title + "</span>" });
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