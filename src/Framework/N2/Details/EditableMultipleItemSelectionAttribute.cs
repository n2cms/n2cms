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
    /// <summary>
    /// Allows selecting zero or more items of a specific type from a drop down list.
    /// </summary>
    /// <example>
    ///     [EditableMultipleItemSelection]
    ///     public virtual IEnumerable&gt;ContentItem&lt; Links { get; set; }
    /// </example>
    [AttributeUsage(AttributeTargets.Property)]
    public class EditableMultipleItemSelectionAttribute : EditableItemSelectionAttribute
    {
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
            ddl.SearchTreshold = SearchTreshold;
        }

        public override void Write(ContentItem item, string propertyName, System.IO.TextWriter writer)
        {
            var items = item.GetDetailCollection(Name, false);

            if (items != null)
            {
                foreach (var referencedItem in items.OfType<ContentItem>())
                {
                    DisplayableAnchorAttribute.GetLinkBuilder(item, referencedItem, propertyName, null, null).WriteTo(writer);
                }
            }
        }
    }
}
