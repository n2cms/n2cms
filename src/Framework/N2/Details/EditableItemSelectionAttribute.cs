using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Persistence;
using System.Globalization;
using System.Web.UI.WebControls;
using N2.Persistence.NH;
using NHibernate.Linq;
using N2.Definitions;

namespace N2.Details
{
	public class EditableItemSelectionAttribute : EditableDropDownAttribute
	{
		public Type LinkedType { get; protected set; }
		public Type ExcludedType { get; set; }
		public int SearchTreshold { get; set; }
		public bool IncludePages { get; set; }
		public bool IncludeParts { get; set; }
		
		public EditableItemSelectionAttribute()
		{
			LinkedType = typeof(ContentItem);
			ExcludedType = typeof(ISystemNode);
			SearchTreshold = 20;
			IncludePages = true;
		}

		public EditableItemSelectionAttribute(Type linkedType)
			: this()
		{
			LinkedType = linkedType;
		}

		public EditableItemSelectionAttribute(Type linkedType, string title, int sortOrder)
			: this()
		{
			Title = title;
			SortOrder = sortOrder;
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

		protected virtual IEnumerable<ContentItem> GetDataItemsByIds(params int[] ids)
		{
			var items = Engine.Content.Search.Find.Where
				.Property("ID").In(ids)
				.Select();

			return items;
		}

		public override void UpdateEditor(ContentItem item, System.Web.UI.Control editor)
		{
			var linkedItems = GetStoredSelection(item);

			var checkboxList = editor as ListControl;
			if (checkboxList == null) return;

			foreach (ListItem checkboxItem in checkboxList.Items)
			{
				int checkboxValue;
				if (int.TryParse(checkboxItem.Value, out checkboxValue))
				{
					checkboxItem.Selected = linkedItems.Contains(checkboxValue);
				}
			}
		}

		public override bool UpdateItem(ContentItem item, System.Web.UI.Control editor)
		{
			var checkboxList = editor as ListControl;
			if (checkboxList == null) return false;

			// Get a map of currently linked items
			var storedItems = GetStoredSelection(item);

			// Get a map of all selected items from UI
			var selectedLinkedItems = (from ListItem checkboxItem in checkboxList.Items
																 where checkboxItem.Selected
																 select (int)ConvertToValue(checkboxItem.Value)).ToArray();

			// Check whether there were any changes
			var hasChanges = storedItems.Any(selectedLinkedItems.Contains) == false 
				|| selectedLinkedItems.Any(storedItems.Contains) == false 
				|| storedItems.Count != selectedLinkedItems.Length;

			// Only hook up items when there were changes
			if (hasChanges)
			{
				// Convert id array to ContentItems
				var linksToAdd = GetDataItemsByIds(selectedLinkedItems);

				ReplaceStoredValue(item, linksToAdd);
			}

			return hasChanges;
		}

		protected virtual void ReplaceStoredValue(ContentItem item, IEnumerable<ContentItem> linksToReplace)
		{
			item[Name] = linksToReplace.FirstOrDefault();
		}

		protected virtual HashSet<int> GetStoredSelection(ContentItem item)
		{
			var storedSelection = new HashSet<int>();
			
			var referencedItem = item[Name] as ContentItem;
			if (referencedItem != null)
				storedSelection.Add(referencedItem.ID);

			return storedSelection;
		}
	}
}
