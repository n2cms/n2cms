using System;
using System.Collections.Generic;

namespace N2.Persistence
{
	/// <summary>Classes implementing this interface can be used to find relations in N2. A relation exists when an item's details contains another item.</summary>
	/// <example>
	/// <![CDATA[
	///		// Create a relation
	///		referencingItem["friend"] = referencedItem;
	/// 
	///		// Find referencing items
	///		IRelationFinder finder = N2.Factory.Persister.Finder; // Get a finder
	///		finder.SetDetailName("friend"); // Only relations with a certain name
	///		finder.SetSortExpression("Published DESC"); // sorting
	///		ContentItem referencedItem = CurrentItem; // this is the item whose referring items we're interested in
	///		foreach(ContentItem referencingItem in finder.ListReferrers(referencedItem))
	///			; // do something
	/// ]]>
	/// </example>
	public interface IRelationFinder
	{
		/// <summary>Sets the name of references we want to find. If this setting isn't set all references are serached (defualt).</summary>
		/// <param name="detailName">The name of the detail to look for.</param>
		/// <returns>The same relation finder.</returns>
		IRelationFinder SetDetailName(string detailName);
		
		/// <summary>Wether previous versions referencing also should be included in the results. Defaults to false.</summary>
		/// <param name="include">Set to true to get hits from previous versions.</param>
		/// <returns>The same relation finder.</returns>
		IRelationFinder SetIncludePreviousVersions(bool include);

		/// <summary>Finds references, i.e. the objects used to store relations.</summary>
		/// <param name="targetItem">The target of the references we'd like to find.</param>
		/// <returns>A list of the details referencing the supplied item.</returns>
		IList<Details.LinkDetail> ListReferences(ContentItem targetItem);
		
		/// <summary>Finds items referencing the supplied item.</summary>
		/// <param name="targetItem">The item that is the target of references from the items we want to find.</param>
		/// <returns>A list of items referencing the supplied item.</returns>
		IList<ContentItem> ListReferrers(ContentItem targetItem);

		/// <summary>Sets wether the search result may be cached. Defaults to true.</summary>
		/// <param name="enableCache">True if the query can be cached.</param>
		/// <returns>The same relation finder.</returns>
		IRelationFinder SetCachable(bool enableCache);

		/// <summary>The first item in the list to return. This is used for paging. Defaults to 0.</summary>
		/// <param name="firstResult">The first result to return.</param>
		/// <returns>The same relation finder.</returns>
		IRelationFinder SetFirstResult(int firstResult);

		/// <summary>The maximum number of items in the list to return. This is used for paging. Defaults to infinite.</summary>
		/// <param name="maxResult">The number of items to return.</param>
		/// <returns>The same relation finder.</returns>
		IRelationFinder SetMaxResults(int maxResult);

		/// <summary>Sets the sort expression to use with the query. The expression is SQL-inspired, e.g. "Published DESC"</summary>
		/// <param name="sortExpression">The sort expression to use.</param>
		/// <returns>The same relation finder.</returns>
		IRelationFinder SetSortExpression(string sortExpression);

	}
}
