using System;
using System.Linq;
using System.Collections.Generic;
using N2.Engine;
using NHibernate;
using NHibernate.Criterion;

namespace N2.Persistence.NH
{
	[Service(typeof(IContentItemRepository), Key = "n2.ContentItemRepository")]
	[Service(typeof(IRepository<ContentItem>), Key = "n2.repository.ContentItem")]
	public class ContentItemRepository : NHRepository<ContentItem>, IContentItemRepository
	{
		public ContentItemRepository(ISessionProvider sessionProvider)
			: base(sessionProvider)
		{
		}

		object zero = 0;
		public override ContentItem Get(object id)
		{
			if (zero.Equals(id)) return null;
			return SessionProvider.OpenSession.Session.Get<ContentItem>(id);
		}

		public override T Get<T>(object id)
		{
			if (zero.Equals(id)) return default(T);
			return SessionProvider.OpenSession.Session.Get<T>(id);
		}

		ICriteria GetEagerContentItemCriteria()
		{
			return GetContentItemCriteria()
				.SetFetchMode("ci.Children", FetchMode.Eager)
				.SetFetchMode("ci.Details", FetchMode.Eager)
				.SetFetchMode("ci.DetailCollections", FetchMode.Eager);
		}

		private ICriteria GetContentItemCriteria()
		{
			return SessionProvider.OpenSession.Session.CreateCriteria(typeof(ContentItem), "ci");
		}

		/// <summary>Gets types of items below a certain item.</summary>
		/// <param name="ancestor">The root level item to include in the search.</param>
		/// <returns>An enumeration of discriminators and number of items with that discriminator.</returns>
		public IEnumerable<DiscriminatorCount> FindDescendantDiscriminators(ContentItem ancestor)
		{
			IQuery query;
			if (ancestor == null)
				query = SessionProvider.OpenSession.Session
					.CreateQuery("select ci.class, count(*) from ContentItem ci group by ci.class");
			else
				query = SessionProvider.OpenSession.Session
					.CreateQuery("select ci.class, count(*) from ContentItem ci where ci.ID=:id or ci.AncestralTrail like :trail group by ci.class")
					.SetParameter("id", ancestor.ID)
					.SetParameter("trail", ancestor.GetTrail() + "%");

			return query.Enumerable()
				.OfType<object[]>()
				.Select(row => new DiscriminatorCount { Discriminator = (string)row[0], Count = (int)(long)row[1] })
				.OrderByDescending(dc => dc.Count);
		}

		/// <summary>Finds published items below a certain ancestor of a specific type.</summary>
		/// <param name="ancestor">The ancestor whose descendants are searched.</param>
		/// <param name="discriminator">The discriminator the are filtered by.</param>
		/// <returns>An enumeration of items matching the query.</returns>
		public IEnumerable<ContentItem> FindDescendants(ContentItem ancestor, string discriminator)
		{
			if (ancestor == null)
				return SessionProvider.OpenSession.Session
					.CreateQuery("select ci from ContentItem ci where ci.class=:class")
					.SetParameter("class", discriminator)
					.Enumerable<ContentItem>();
			else
				return SessionProvider.OpenSession.Session
					.CreateQuery("select ci from ContentItem ci where (ci.ID=:id or ci.AncestralTrail like :trail) and ci.class=:class")
					.SetParameter("id", ancestor.ID)
					.SetParameter("trail", ancestor.GetTrail() + "%")
					.SetParameter("class", discriminator)
					.Enumerable<ContentItem>();
		}
	}
}
