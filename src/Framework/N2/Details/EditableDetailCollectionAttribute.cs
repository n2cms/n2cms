using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using N2.Persistence;
using N2.Persistence.NH;
using N2.Web.UI.WebControls;
using NHibernate.Linq;
using N2.Definitions;
using N2.Persistence.Finder;

namespace N2.Details
{
	public class EditableDetailCollectionAttribute : EditableDropDownAttribute
	{
		public Type LinkedType { get; private set; }
		public Type ExcludedType { get; set; }
		public int SearchTreshold { get; set; }
		public bool EvictAfterLoad { get; set; }
		public bool IncludePages { get; set; }
		public bool IncludeParts { get; set; }
		
		public EditableDetailCollectionAttribute()
		{
			PersistAs = PropertyPersistenceLocation.DetailCollection;
			LinkedType = typeof(ContentItem);
			ExcludedType = typeof(ISystemNode);
			SearchTreshold = 20;
			EvictAfterLoad = false;
			IncludePages = true;
		}

		public EditableDetailCollectionAttribute(Type linkedType)
			: this()
		{
			LinkedType = linkedType;
		}

		public EditableDetailCollectionAttribute(Type linkedType, string title, int sortOrder)
			: base(title, sortOrder)
		{
			Title = title;
			SortOrder = sortOrder;
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
			var query = Engine.Content.Search.Find.Where.State.Eq(ContentState.Published);

			if (LinkedType != null && LinkedType != typeof(ContentItem))
				query = query.And.Type.Eq(LinkedType);

			if (ExcludedType != null)
				query = query.And.Type.NotEq(ExcludedType);

			if (IncludePages && !IncludeParts)
				query = query.And.ZoneName.IsNull();
			else if (!IncludePages && IncludeParts)
				query = query.And.ZoneName.IsNull(false);

			var items = query.Select("ID", "Title");

			return items.Select(row => new ListItem((string)row["Title"], row["ID"].ToString()))
				.ToArray();
		}

		protected virtual IList<ContentItem> GetDataItemsByIds(params int[] ids)
		{
			var items = Engine.Content.Search.Find.Where
				.Property("ID").In(ids)
				.Select();

			// Terrible hack to make sure large collections are not persisted after
			// they have been loaded, killing your save performance
			if (EvictAfterLoad)
			{
				var session = Engine.Resolve<ISessionProvider>().OpenSession.Session;
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
																 select (int)ConvertToValue(checkboxItem.Value)).ToArray();

			// Check whether there were any changes
			var hasChanges = linkedItems.Keys.Any(selectedLinkedItems.Contains) == false 
				|| selectedLinkedItems.Any(linkedItems.ContainsKey) == false 
				|| linkedItems.Count != selectedLinkedItems.Length;

			// Only hook up items when there were changes
			if (hasChanges)
			{
				// Convert id array to ContentItems
				var linksToAdd = GetDataItemsByIds(selectedLinkedItems);

				// Replace DetailCollection
				item.GetDetailCollection(Name, true).Replace(linksToAdd);
			}

			return hasChanges;
		}

		protected virtual Dictionary<int, ContentItem> GetCurrentDetailLinks(ContentItem item)
		{
			var detailLinks = item.GetDetailCollection(Name, false);

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