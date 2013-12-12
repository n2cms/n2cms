using System;

namespace N2.Persistence.Finder
{
    /// <summary>
    /// Classes implementing this interface provides methods to define criterias
    /// when building a query.
    /// </summary>
    public interface IQueryBuilder
    {
        /// <summary>Finds items with by ID, effectivly Persister.Get(id).</summary>
        IComparisonCriteria<int> ID { get; }

        /// <summary>Finds items with by it's parent item's ID, effectively Persister.Get(id).GetChildren().</summary>
        IComparisonCriteria<int> ParentID { get; }

        /// <summary>Finds items by parent item, effectively Persister.Get(id).GetChildren()</summary>
        IComparisonCriteria<ContentItem> Parent { get; }

        /// <summary>Find items with a certain title.</summary>
        IStringCriteria Title { get; }

        /// <summary>Find items with a certain name.</summary>
        IStringCriteria Name { get; }

        /// <summary>Find items with a certain zone name.</summary>
        IStringCriteria ZoneName { get; }

        /// <summary>Find items with a certain created date.</summary>
        IComparisonCriteria<DateTime> Created { get; }

        /// <summary>Find items with a certain updated date.</summary>
        IComparisonCriteria<DateTime> Updated { get; }

        /// <summary>Find items with a certain published date.</summary>
        INullableComparisonCriteria<DateTime> Published { get; }

        /// <summary>Find items with a certain expiration date.</summary>
        INullableComparisonCriteria<DateTime> Expires { get; }

        /// <summary>Find items with a certain sort order.</summary>
        IComparisonCriteria<int> SortOrder { get; }

        /// <summary>Find items with a certain version index.</summary>
        IComparisonCriteria<int> VersionIndex { get; }

        /// <summary>Find items with a certain version index.</summary>
        IComparisonCriteria<ContentState> State { get; }
        
        /// <summary>Find items with a certain visibility.</summary>
        ICriteria<bool> Visible { get; }

        [Obsolete("Querying version is no longer supported")]
        /// <summary>Find previous versions of an item.</summary>
        ICriteria<ContentItem> VersionOf { get; }

        /// <summary>Find items by who saved them.</summary>
        IStringCriteria SavedBy { get; }

        /// <summary>Find items by location in hierarchy.</summary>
        IStringCriteria AncestralTrail { get; }

        /// <summary>Find items having details matching the criteria and a certain name .</summary>
        IDetailCriteria Detail(string name);

        /// <summary>Find items having a detail matching the criteria.</summary>
        IDetailCriteria Detail();

        /// <summary>"Opens a bracket" i.e. begins a group of criterias that are evaluated together. Must be followed by a CloseBracket.</summary>
        IQueryBuilder OpenBracket();

        /// <summary>Finds items belonging to a certain type. Note that derived types are not found with this method.</summary>
        ICriteria<Type> Type { get; }

        /// <summary>Finds items with a [Persistable] property named.</summary>
        /// <param name="propertyName">The name of the property.</param>
        IPropertyCriteria Property(string persistablePropertyName);
    }
}
