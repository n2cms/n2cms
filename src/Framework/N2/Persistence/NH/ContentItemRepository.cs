using System.Collections.Generic;
using System.Linq;
using N2.Details;
using N2.Engine;
using NHibernate;

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

		private ISession Session
		{
			get { return SessionProvider.OpenSession.Session; }
		}

		object zero = 0;
		public override ContentItem Get(object id)
		{
			if (id == zero) return null;
			return SessionProvider.OpenSession.Session.Get<ContentItem>(id);
		}

		public override T Get<T>(object id)
		{
			if (id == zero) return default(T);
			return SessionProvider.OpenSession.Session.Get<T>(id);
		}

		/// <summary>Gets types of items below a certain item.</summary>
		/// <param name="ancestor">The root level item to include in the search.</param>
		/// <returns>An enumeration of discriminators and number of items with that discriminator.</returns>
		public IEnumerable<DiscriminatorCount> FindDescendantDiscriminators(ContentItem ancestor)
		{
			return Session
				.CreateQuery("select ci.class, count(*) from ContentItem ci where ci.ID=:id or ci.AncestralTrail like :trail group by ci.class order by count(*) desc")
				.SetParameter("id", ancestor.ID)
				.SetParameter("trail", ancestor.GetTrail() + "%")
				.SetCacheable(SessionProvider.CacheEnabled)
				.Enumerable()
				.OfType<object[]>()
				.Select(row => new DiscriminatorCount { Discriminator = (string)row[0], Count = (int)(long)row[1] });
		}

		/// <summary>Finds published items below a certain ancestor of a specific type.</summary>
		/// <param name="ancestor">The ancestor whose descendants are searched.</param>
		/// <param name="discriminator">The discriminator the are filtered by.</param>
		/// <returns>An enumeration of items matching the query.</returns>
		public IEnumerable<ContentItem> FindDescendants(ContentItem ancestor, string discriminator)
		{
			return Session
				.CreateQuery("select ci from ContentItem ci where (ci.ID=:id or ci.AncestralTrail like :trail) and ci.class=:class")
				.SetParameter("id", ancestor.ID)
				.SetParameter("trail", ancestor.GetTrail() + "%")
				.SetParameter("class", discriminator)
				.SetCacheable(SessionProvider.CacheEnabled)
				.Enumerable<ContentItem>();
		}

		public IEnumerable<ContentItem> FindReferencing(ContentItem linkTarget)
		{
			return Session
				.CreateQuery("select ci from ContentItem ci where ci.ID in (select cd.EnclosingItem.ID from ContentDetail cd where cd.LinkedItem=:target)")
				.SetParameter("target", linkTarget)
				.SetCacheable(SessionProvider.CacheEnabled)
				.Enumerable<ContentItem>();
		}

		public int RemoveReferencesToRecursive(ContentItem target)
		{
			var s = Session;
			var details = s.CreateQuery("from cd in ContentDetail where (cd.LinkedItem = :ancestor or cd.LinkedItem.AncestralTrail like :trail)")
				.SetParameter("ancestor", target)
				.SetParameter("trail", target.GetTrail())
				.Enumerable<ContentDetail>();

			int count = 0;
			foreach (var cd in details)
			{
				cd.AddTo((ContentItem)null);
				s.Delete(cd);
				count++;
			}
			s.Flush();

			return count;
		}
	}

}
