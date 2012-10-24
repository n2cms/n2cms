using System.Collections.Generic;
using System.Linq;
using N2.Details;
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
				.CreateQuery("select ci from ContentItem ci where ci.ID in (select cd.EnclosingItem.ID from ContentDetail cd where cd.LinkedItem=:target)")
				.SetParameter("target", linkTarget)
				.SetCacheable(SessionProvider.CacheEnabled)
				.Enumerable<ContentItem>();
		}

		//private void DeleteReferencesRecursive(ContentItem itemNoMore)
		//{
		//    string itemTrail = Utility.GetTrail(itemNoMore);
		//    var inboundLinks = Find.EnumerateChildren(itemNoMore, true, false)
		//        .SelectMany(i => linkRepository.Find(new Parameter("LinkedItem", i), new Parameter("ValueTypeKey", ContentDetail.TypeKeys.LinkType)))
		//        .Where(l => !Utility.GetTrail(l.EnclosingItem).StartsWith(itemTrail))
		//        .ToList();

		//    Engine.Logger.Info("ContentPersister.DeleteReferencesRecursive " + inboundLinks.Count + " of " + itemNoMore);

		//    foreach (ContentDetail link in inboundLinks)
		//    {
		//        linkRepository.Delete(link);
		//        link.AddTo((DetailCollection)null);
		//    }
		//    linkRepository.Flush();
		//}

		public int RemoveReferencesToRecursive(ContentItem target)
		{
			var s = Session;
			var details = s.CreateQuery("from cd in ContentDetail where (cd.LinkedItem = :ancestor or cd.LinkedItem.AncestralTrail like :trail)")
				.SetParameter("ancestor", target)
				.SetParameter("trail", target.GetTrail() + "%")
				.List<ContentDetail>();

			HashSet<ContentItem> toUpdate = new HashSet<ContentItem>();
			int count = 0;
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

		protected override ICriterion CreateCriterion(IParameter parameter)
		{
			var p = parameter as Parameter;
			if (p != null && p.IsDetail)
			    return CreateDetailExpression(p);
			
			return base.CreateCriterion(parameter);
		}

		private ICriterion CreateDetailExpression(Parameter p)
		{
			return Subqueries.PropertyIn("ID",
				DetachedCriteria.For<ContentDetail>()
					.SetProjection(Projections.Property("EnclosingItem.ID"))
					.Add(Expression.Eq("Name", p.Name))
					.Add(CreateExpression(ContentDetail.GetAssociatedPropertyName(p.Value), p.Value, p.Comparison)));
		}
	}

}
