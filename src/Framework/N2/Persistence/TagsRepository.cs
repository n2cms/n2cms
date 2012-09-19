using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine;
using N2.Persistence.Proxying;
using N2.Linq;
using N2.Persistence.NH;

namespace N2.Persistence
{
	/// <summary>
	/// Supports saving and retrieving tags from content items.
	/// </summary>
	[Service]
	public class TagsRepository
	{
		IEngine engine;

		public TagsRepository(IEngine engine)
		{
			this.engine = engine;
		}

		/// <summary>Finds all stored tags.</summary>
		/// <param name="ancestor"></param>
		/// <param name="tagGroup"></param>
		/// <returns></returns>
		public virtual IEnumerable<string> FindTags(ContentItem ancestor, string tagGroup)
		{
			return engine.QueryItems()
				.WherePublished()
				.WhereDescendantOrSelf(ancestor)
				.SelectMany(i => i.DetailCollections)
				.Where(dc => dc.Name == tagGroup)
				.SelectMany(dc => dc.Details)
				.Select(d => d.StringValue)
				.Distinct()
				.ToList();
		}

		/// <summary>Finds all stored tags.</summary>
		/// <param name="ancestor"></param>
		/// <param name="tagGroup"></param>
		/// <returns></returns>
		public virtual IEnumerable<KeyValuePair<string, int>> FindTagSizes(ContentItem ancestor, string tagGroup)
		{
			var details = engine.QueryItems()
				.WherePublished()
				.WhereDescendantOrSelf(ancestor)
				.SelectMany(i => i.DetailCollections)
				.Where(dc => dc.Name == tagGroup)
				.SelectMany(dc => dc.Details);
			return details.GroupBy(d => d.StringValue)
				.Select(g => new { g.Key, Count = g.Count() })
				.ToList()
				.Select(g => new KeyValuePair<string, int>(g.Key, g.Count));
		}

		/// <summary>Finds items with a certain tag.</summary>
		/// <param name="ancestor"></param>
		/// <param name="tagGroup"></param>
		/// <returns></returns>
		public virtual IEnumerable<ContentItem> FindTagged(ContentItem ancestor, string tagGroup, string tagName)
		{
			return engine.QueryDetails()
				.Where(d => d.Name == tagGroup)
				.Where(d => d.StringValue == tagName)
				.Select(d => d.EnclosingItem)
				.WherePublished()
				.WhereDescendantOrSelf(ancestor)
				.Distinct()
				.ToList();
		}

		/// <summary>Gets tags on a given item.</summary>
		/// <param name="item"></param>
		/// <param name="tagGroup"></param>
		/// <returns></returns>
		public virtual IEnumerable<string> GetTags(ContentItem item, string tagGroup)
		{
			return GetTagsFromValues(item, tagGroup);
		}

		/// <summary>Gets tags on a given item.</summary>
		/// <param name="item"></param>
		/// <param name="tagGroup"></param>
		/// <returns></returns>
		public static IEnumerable<string> GetTagsFromValues(IInterceptableType item, string tagGroup)
		{
			return ((item as IInterceptableType).GetValues(tagGroup) ?? new string[0]).OfType<string>();
		}

		/// <summary>Sets tags on a given item without saving it.</summary>
		/// <param name="item"></param>
		/// <param name="tagGroup"></param>
		/// <param name="rows"></param>
		public virtual void SetTags(ContentItem item, string tagGroup, IEnumerable<string> rows)
		{
			(item as IInterceptableType).SetValues(tagGroup, rows);
		}

		/// <summary>Sets tags on a given item and saves it.</summary>
		/// <param name="item"></param>
		/// <param name="tagGroup"></param>
		/// <param name="rows"></param>
		public virtual void SaveTags(ContentItem item, string tagGroup, IEnumerable<string> rows)
		{
			SetTags(item, tagGroup, rows);
			engine.Persister.Save(item);
		}
	}
}
