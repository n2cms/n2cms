using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using N2.Persistence;
using N2.Persistence.NH;
using N2.Web.UI.WebControls;
using NHibernate.Linq;

namespace N2.Details
{
	public class EditableDetailCollectionAttribute : EditableDropDownAttribute
	{
		public Type LinkedType { get; private set; }
		public string DetailCollection { get; private set; }
		public int SearchTreshold { get; set; }
		public bool EvictAfterLoad { get; set; }

		public EditableDetailCollectionAttribute(string detailCollection)
		{
			PersistAs = PropertyPersistenceLocation.DetailCollection;
			LinkedType = typeof(ContentItem);
			DetailCollection = detailCollection;
			Name = detailCollection;
			SearchTreshold = 20;
			EvictAfterLoad = true;
		}

		public EditableDetailCollectionAttribute(Type linkedType, string detailCollection)
		{
			PersistAs = PropertyPersistenceLocation.DetailCollection;
			LinkedType = linkedType;
			DetailCollection = detailCollection;
			Name = detailCollection;
			SearchTreshold = 20;
			EvictAfterLoad = true;
		}

		public EditableDetailCollectionAttribute(Type linkedType, string detailCollection, string title, int sortOrder)
			: base(title, sortOrder)
		{
			PersistAs = PropertyPersistenceLocation.DetailCollection;
			LinkedType = linkedType;
			DetailCollection = detailCollection;
			Name = detailCollection;
			SearchTreshold = 20;
			EvictAfterLoad = true;
		}

		protected override System.Web.UI.Control AddEditor(System.Web.UI.Control container)
		{
			var multiSelect = new MultiSelect { ID = Name };

			multiSelect.Items.AddRange(GetListItems());
			Configure(multiSelect);
			container.Controls.Add(multiSelect);
			return multiSelect;
		}

		protected virtual void Configure(MultiSelect ddl)
		{
			ddl.EnableFilter = SearchTreshold >= 0 && ddl.Items.Count >= SearchTreshold;
			ddl.SelectedList = byte.MaxValue;
		}

		protected override string GetValue(ContentItem item)
		{
			return item.ID.ToString(CultureInfo.InvariantCulture);
		}

		protected override object ConvertToValue(string value)
		{
			int id;
			if (!int.TryParse(value, out id))
			{
				throw new Exception(string.Format("Invalid id: {0}", value));
			}

			return id;
		}

		protected override ListItem[] GetListItems()
		{
			var items = LinkedType == null
											? new ContentItem[0]
											: GetContentItemsGetter().Invoke();

			return items
				.Select(item =>

								new ListItem(
									item.Title,
									item.ID.ToString(CultureInfo.InvariantCulture)))

				.ToArray();
		}

		protected virtual Func<IEnumerable<ContentItem>> GetContentItemsGetter()
		{
			return GetDataItems;
		}

		protected virtual Func<int[], IEnumerable<ContentItem>> GetContentItemsByIdGetter()
		{
			return GetDataItemsByIds;
		}

		protected virtual IList<ContentItem> GetDataItems()
		{
			var items = Find.Items.Where
				.Type.Eq(LinkedType)
				.And
				.State.Eq(ContentState.Published)
				.Select();

			// Terrible hack to make sure large collections are not persisted after
			// they have been loaded, killing your save performance
			if (EvictAfterLoad)
			{
				var session = Context.Current.Resolve<ISessionProvider>().OpenSession.Session;
				items.ForEach(session.Evict);
			}

			return items;
		}

		protected virtual IList<ContentItem> GetDataItemsByIds(params int[] ids)
		{
			var items = Find.Items.Where
				.Property("ID").In(ids)
				.Select();

			// Terrible hack to make sure large collections are not persisted after
			// they have been loaded, killing your save performance
			if (EvictAfterLoad)
			{
				var session = Context.Current.Resolve<ISessionProvider>().OpenSession.Session;
				items.ForEach(session.Evict);
			}

			return items;
		}

		public override void UpdateEditor(ContentItem item, System.Web.UI.Control editor)
		{
			var linkedItems = GetCurrentDetailLinks(item);

			var checkboxList = editor as MultiSelect;
			if (checkboxList == null) return;

			Configure(checkboxList);

			foreach (ListItem checkboxItem in checkboxList.Items)
			{
				int checkboxValue;
				if (int.TryParse(checkboxItem.Value, out checkboxValue))
				{
					checkboxItem.Selected = linkedItems.ContainsKey(checkboxValue);
				}
			}
		}

		public override bool UpdateItem(ContentItem item, System.Web.UI.Control editor)
		{
			var checkboxList = editor as MultiSelect;
			if (checkboxList == null) return false;

			// Get a map of currently linked items
			var linkedItems = GetCurrentDetailLinks(item);

			// Get a map of all selected items from UI
			var selectedLinkedItems = (from ListItem checkboxItem in checkboxList.Items
																 where checkboxItem.Selected
																 select (int)ConvertToValue(checkboxItem.Value)).ToList();

			// Check whether there were any changes
			var hasChanges =
				linkedItems.Keys.Any(selectedLinkedItems.Contains) == false ||
				selectedLinkedItems.Any(linkedItems.ContainsKey) == false;

			// Only hook up items when there were changes
			if (hasChanges)
			{
				// Convert id array to ContentItems
				var linksToAdd = GetDataItemsByIds(selectedLinkedItems.ToArray());

				// Replace DetailCollection
				item.GetDetailCollection(DetailCollection, true).Replace(linksToAdd);
			}

			return hasChanges;
		}

		protected virtual Dictionary<int, ContentItem> GetCurrentDetailLinks(ContentItem item)
		{
			var detailLinks = item.GetDetailCollection(DetailCollection, false);

			var childrenDictionary = new Dictionary<int, ContentItem>();
			if (detailLinks != null)
			{
				foreach (ContentItem linkedItem in detailLinks)
				{
					childrenDictionary[linkedItem.ID] = linkedItem;
				}
			}
			return childrenDictionary;
		}

	}
}