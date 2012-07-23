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
	public class EditableMultipleItemSelectionAttribute : EditableItemSelectionAttribute
	{
		public bool EvictAfterLoad { get; set; }

		public EditableMultipleItemSelectionAttribute()
		{
			PersistAs = PropertyPersistenceLocation.DetailCollection;
		}

		public EditableMultipleItemSelectionAttribute(Type linkedType)
			: this()
		{
			LinkedType = linkedType;
		}

		public EditableMultipleItemSelectionAttribute(Type linkedType, string title, int sortOrder)
			: this()
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

		protected override IEnumerable<ContentItem> GetDataItemsByIds(params int[] ids)
		{
			var items = base.GetDataItemsByIds(ids);

			// Terrible hack to make sure large collections are not persisted after
			// they have been loaded, killing your save performance
			if (EvictAfterLoad)
			{
				var session = Engine.Resolve<ISessionProvider>().OpenSession.Session;
				items.ForEach(session.Evict);
			}

			return items;
		}

		protected override HashSet<int> GetStoredSelection(ContentItem item)
		{
			var detailLinks = item.GetDetailCollection(Name, false);

			if (detailLinks == null)
				return new HashSet<int>();

			return new HashSet<int>(detailLinks.Details.Where(d => d.LinkValue.HasValue).Select(d => d.LinkValue.Value));
		}

		protected override void ReplaceStoredValue(ContentItem item, IEnumerable<ContentItem> linksToReplace)
		{
			item.GetDetailCollection(Name, true).Replace(linksToReplace);
		}

		protected virtual void Configure(MultiSelect ddl)
		{
			ddl.EnableFilter = SearchTreshold >= 0 && ddl.Items.Count >= SearchTreshold;
			ddl.SelectedList = byte.MaxValue;
		}

	}
}