using System.Collections.Generic;
using System.Linq;
using N2.Details;
using N2.Engine;
using NHibernate;
using NHibernate.Criterion;

namespace N2.Persistence.NH
{
	[Service(typeof(IContentItemRepository), Configuration = "sql", Key = "n2.ContentItemRepository")]
	[Service(typeof(IRepository<ContentItem>), Configuration = "sql", Key = "n2.repository.ContentItem")]
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
            if (zero.Equals(id)) return null;
            return SessionProvider.OpenSession.Session.Get<ContentItem>(id);
        }

        public override T Get<T>(object id)
        {
            if (zero.Equals(id)) return default(T);
            return SessionProvider.OpenSession.Session.Get<T>(id);
        }

        /// <summary>Gets types of items below a certain item.</summary>
        /// <param name="ancestor">The root level item to include in the search.</param>
        /// <returns>An enumeration of discriminators and number of items with that discriminator.</returns>
        public IEnumerable<DiscriminatorCount> FindDescendantDiscriminators(ContentItem ancestor)
        {
            IQuery query;
            if(ancestor == null)
                query = SessionProvider.OpenSession.Session
                    .CreateQuery("select ci.class, count(*) from ContentItem ci group by ci.class");
            else
                query = SessionProvider.OpenSession.Session
                    .CreateQuery("select ci.class, count(*) from ContentItem ci where ci.ID=:id or ci.AncestralTrail like :trail group by ci.class")
                    .SetParameter("id", ancestor.ID)
                    .SetParameter("trail", ancestor.GetTrail() + "%");

            return query.SetCacheable(SessionProvider.CacheEnabled)
                .Enumerable()
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
                    .SetCacheable(SessionProvider.CacheEnabled)
                    .Enumerable<ContentItem>();
        }

        public IEnumerable<ContentItem> FindReferencing(ContentItem linkTarget)
        {
            return Session
                .CreateQuery("select ci from ContentItem ci where ci.ID in (select cd.EnclosingItem.ID from ContentDetail cd where cd.LinkedItem.ID=:targetID)")
                .SetParameter("targetID", linkTarget.ID)
                .SetCacheable(SessionProvider.CacheEnabled)
                .Enumerable<ContentItem>();
        }

        public virtual int RemoveReferencesToRecursive(ContentItem target)
        {
            var s = Session;
            var details = s.CreateQuery("from cd in ContentDetail where cd.LinkedItem.ID = :ancestorID or cd.LinkedItem.ID in (select ci.ID from ContentItem ci where ci.AncestralTrail like :trail)")
                .SetParameter("ancestorID", target.ID)
                .SetParameter("trail", target.GetTrail() + "%")
                .List<ContentDetail>();

            var toUpdate = new HashSet<ContentItem>();
            var count = 0;
            foreach (var cd in details)
            {
                toUpdate.Add(cd.EnclosingItem);
                cd.AddTo((ContentItem)null);
                count++;
            }
            foreach (var item in toUpdate)
                s.Update(item);

            s.Flush();

            return count;
        }
    }

}
