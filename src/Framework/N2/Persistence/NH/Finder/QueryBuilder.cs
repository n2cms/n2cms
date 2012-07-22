using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using N2.Collections;
using N2.Definitions;
using N2.Definitions.Static;
using N2.Persistence.Finder;
using NHibernate;
using System.Linq.Expressions;

namespace N2.Persistence.NH.Finder
{
	/// <summary>
	/// The query builder stores query settings and can perform selects on the
	/// database.
	/// </summary>
	[Serializable]
	public class QueryBuilder : IQueryBuilder, IQueryAction
	{
		#region Fields

		[NonSerialized] private ISessionProvider sessionProvider;
		[NonSerialized] private DefinitionMap map;

		private IList<IHqlProvider> criterias = new List<IHqlProvider>();
		private Operator currentOperator = Operator.None;
		private VersionOption versions = VersionOption.Exclude;
		private bool cachable = true;
		private string sortExpression = null;
		private string orderBy = null;
		private IList<ItemFilter> filters = new List<ItemFilter>();
		private int firstResult = 0;
		private int maxResults = 0;

		#endregion

		#region Constructor

		public QueryBuilder(ISessionProvider sessionProvider, DefinitionMap map)
		{
			this.sessionProvider = sessionProvider;
			this.map = map;
		}

		#endregion

		#region Properties

		/// <summary>Gets or sets the NHibernate session provider.</summary>
		public ISessionProvider SessionProvider
		{
			get { return sessionProvider; }
			set { sessionProvider = value; }
		}

		/// <summary>Gets or sets wether the query should be cached.</summary>
		public bool Cachable
		{
			get { return cachable; }
			set { cachable = value; }
		}

		/// <summary>Gets or sets wether previous versions should be included in the query.</summary>
		public VersionOption Versions
		{
			get { return versions; }
			set { versions = value; }
		}

		/// <summary>Gets or sets the current operator (this is updated as the query is progressed using And or Or).</summary>
		public Operator CurrentOperator
		{
			get { return currentOperator; }
			set { currentOperator = value; }
		}

		/// <summary>Gets the current set of criterias.</summary>
		public IList<IHqlProvider> Criterias
		{
			get { return criterias; }
		}

		/// <summary>Gets or sets filters applied to the result set after it has been fetched from the database.</summary>
		public IList<ItemFilter> Filters
		{
			get { return filters; }
			set { filters = value; }
		}

		/// <summary>Order expression applied to the sql statement.</summary>
		public string OrderBy
		{
			get { return orderBy; }
			set { orderBy = value; }
		}

		/// <summary>Gets or sets the sort expression performed on the result list after selection (not in database).</summary>
		public string SortExpression
		{
			get { return sortExpression; }
			set { sortExpression = value; }
		}

		/// <summary>Gets or sets the maximum number of results to return.</summary>
		public int MaxResults
		{
			get { return maxResults; }
			set { maxResults = value; }
		}

		/// <summary>Gets or sets the first result of the result set to return (skipping those before).</summary>
		public int FirstResult
		{
			get { return firstResult; }
			set { firstResult = value; }
		}

		#endregion

		#region IQueryBase Members

		public IComparisonCriteria<int> ID
		{
			get { return new PropertyCriteria<int>(this, "ID"); }
		}

		public IComparisonCriteria<int> ParentID
		{
			get { return new PropertyCriteria<int>(this, "Parent.ID"); }
		}

		public IComparisonCriteria<ContentItem> Parent
		{
			get { return new PropertyCriteria<ContentItem>(this, "Parent"); }
		}

		public IStringCriteria Title
		{
			get { return new PropertyStringCriteria(this, "Title"); }
		}

		public IStringCriteria Name
		{
			get { return new PropertyStringCriteria(this, "Name"); }
		}

		public IStringCriteria ZoneName
		{
			get { return new PropertyStringCriteria(this, "ZoneName"); }
		}

		public IComparisonCriteria<DateTime> Created
		{
			get { return new PropertyCriteria<DateTime>(this, "Created"); }
		}

		public IComparisonCriteria<DateTime> Updated
		{
			get { return new PropertyCriteria<DateTime>(this, "Updated"); }
		}

		public INullableComparisonCriteria<DateTime> Published
		{
            get { return new NullablePropertyCriteria<DateTime>(this, "Published"); }
		}

        public INullableComparisonCriteria<DateTime> Expires
		{
            get { return new NullablePropertyCriteria<DateTime>(this, "Expires"); }
		}

		public IComparisonCriteria<int> SortOrder
		{
			get { return new PropertyCriteria<int>(this, "SortOrder"); }
        }

        public IComparisonCriteria<int> VersionIndex
        {
            get { return new PropertyCriteria<int>(this, "VersionIndex"); }
        }

        public IComparisonCriteria<ContentState> State
        {
            get { return new PropertyCriteria<ContentState>(this, "State"); }
        }

		public ICriteria<bool> Visible
		{
			get { return new PropertyCriteria<bool>(this, "Visible"); }
		}

		public ICriteria<ContentItem> VersionOf
		{
			get { return new PropertyVersionOfCriteria(this); }
		}

		public IStringCriteria SavedBy
		{
			get { return new PropertyStringCriteria(this, "SavedBy"); }
		}

		public IStringCriteria AncestralTrail
		{
			get { return new PropertyStringCriteria(this, "AncestralTrail"); }
		}

		public IDetailCriteria Detail(string name)
		{
			return new DetailCriteria(this, name);
		}

		public IDetailCriteria Detail()
		{
			return new DetailCriteria(this, null);
		}

		public IQueryBuilder OpenBracket()
		{
			Criterias.Add(new TextOnlyHqlProvider(string.Empty, CurrentOperator, "("));
			CurrentOperator = Operator.None;
			return this;
		}

		public ICriteria<Type> Type
		{
			get { return new PropertyClassCriteria(this); }
		}

		public IPropertyCriteria Property(string persistablePropertyName)
		{
			return new PersistablePropertyCriteria(this, persistablePropertyName);
		}

		#endregion

		#region Methods

		public string GetDiscriminator(Type value)
		{
			if (value == null) throw new ArgumentNullException("value");

			ItemDefinition definition = map.GetOrCreateDefinition(value);
			if (definition == null)
				throw new ArgumentException("Could not find the definition associated with the type '" + value.FullName + "'. Please ensure this is a non-abstract class deriving from N2.ContentItem and that it is decorated by the [Definition] attribute.");
			return definition.Discriminator;
		}

		public IEnumerable<string> GetDiscriminators(Type value)
		{
			return map.GetDefinitions().Where(d => value.IsAssignableFrom(d.ItemType)).Select(d => d.Discriminator).Distinct();
		}

		const string selectHql = "select ci from ContentItem ci";

		protected virtual IQuery CreateQuery()
		{
			return CreateQuery(selectHql);
		}

		protected virtual IQuery CreateQuery(string selectFrom, int skip, int take, bool cacheable)
		{

			StringBuilder from = new StringBuilder(selectFrom, 256);
			StringBuilder where = new StringBuilder(128);

			using (GetAppender(where))
			{
				for (int i = 0; i < criterias.Count; i++)
				{
					criterias[i].AppendHql(from, where, i);
				}
			}

			from.Append(where);
			if (OrderBy != null)
				from.Append(" order by ").Append(OrderBy);

			string hql = from.ToString();
			IQuery query = sessionProvider.OpenSession.Session.CreateQuery(hql);

			for (int i = 0; i < criterias.Count; i++)
			{
				criterias[i].SetParameters(query, i);
			}

			if (skip > 0)
				query.SetFirstResult(skip);
			if (take > 0)
				query.SetMaxResults(take);
			query.SetCacheable(cacheable);

			return query;
		}

		private IQuery CreateQuery(string selectFrom)
		{
			return CreateQuery(selectFrom, FirstResult, MaxResults, Cachable);
		}

		private StringWrapper GetAppender(StringBuilder where)
		{
			if (Versions == VersionOption.Include)
			{
				if (Criterias.Count == 0)
					return new StringWrapper();
				else
					return new StringWrapper(where, " where", null);
			}
			else
			{
				if (Criterias.Count == 0)
					return new StringWrapper(where, " where VersionOfID Is Null", null);
				else
					return new StringWrapper(where, " where VersionOfID Is Null and (", ")");
			}
		}

		#endregion

		#region IQueryAction Members

		public IQueryBuilder And
		{
			get
			{
				CurrentOperator = Operator.And;
				return this;
			}
		}

		public IQueryBuilder Or
		{
			get
			{
				CurrentOperator = Operator.Or;
				return this;
			}
		}

		public IQueryAction CloseBracket()
		{
			Criterias.Add(new TextOnlyHqlProvider(string.Empty, Operator.None, ")"));
			return this;
		}

		#endregion

		#region IQueryEnding Members

		IOrderBy IQueryEnding.OrderBy
		{
			get { return new OrderBy(this); }
		}

		IQueryEnding IQueryEnding.PreviousVersions(VersionOption option)
		{
			Versions = option;
			return this;
		}

		IQueryEnding IQueryEnding.FirstResult(int firstResultIndex)
		{
			FirstResult = firstResultIndex;
			return this;
		}

		IQueryEnding IQueryEnding.MaxResults(int maxResults)
		{
			MaxResults = maxResults;
			return this;
		}

		IQueryEnding IQueryEnding.Filters(params ItemFilter[] filters)
		{
			foreach (ItemFilter filter in filters)
				Filters.Add(filter);
			return this;
		}

		IQueryEnding IQueryEnding.Filters(IEnumerable<ItemFilter> filters)
		{
			foreach (ItemFilter filter in filters)
				Filters.Add(filter);
			return this;
		}

		#endregion

		#region IQueryEnd Members

		public virtual int Count()
		{
			if (Filters.Count > 0)
				throw new N2Exception("Cannot use Filters when Count(), sorry.");
			if (MaxResults > 0)
				throw new N2Exception("Cannot use MaxResults with Count(), sorry.");
			if (FirstResult > 0)
				throw new N2Exception("Cannot use FirstResult with Count(), sorry.");

			IQuery q = CreateQuery("select count(*) from ContentItem ci");
			return Convert.ToInt32(q.List()[0]);
		}

		public IList<ContentItem> Select()
		{
			return Select<ContentItem>();
		}

		public virtual IList<T> Select<T>() where T : ContentItem
		{
			var retrievedItems = CreateQuery().List<T>();
			ItemList<T> items;
			if(retrievedItems.Count == 0)
				items = new ItemList<T>();
			else if (Filters != null)
			{
				var filter = AllFilter.Wrap(Filters);
				items = ToListWithFillup<T>(retrievedItems, filter, /*maxRequeries*/10);
			}
			else
				items = new ItemList<T>(retrievedItems);
			
			if (SortExpression != null)
				items.Sort(SortExpression);

			return items;
		}

		/// <summary>Selects items defined by the given criterias and selects only the properties specified by the selector.</summary>
		/// <param name="selector">An object defining which properties on the item to retrieve.</param>
		public virtual IEnumerable<IDictionary<string, object>> Select(params string[] properties)
		{
			if (Filters != null && Filters.Count > 0)
				throw new NotSupportedException("Filters not supported when using selector");

			var props = properties.ToList();
			string selectStatement = "select "
				+ string.Join(", ", props.Select(p => "ci." + p).ToArray())
				+ " from ContentItem ci";

			var results = CreateQuery(selectStatement).Enumerable();
			foreach (object[] row in results)
			{
				var result = props.Select((p, i) => new { p, v = row[i] }).ToDictionary(x => x.p, x => x.v);
				yield return result;
			}
		}

		private ItemList<T> ToListWithFillup<T>(IList<T> retrievedItems, ItemFilter filter, int maxRequeries) where T : ContentItem
		{
			ItemList<T> items = new ItemList<T>();
			int totalRetrievedItems = 0;
			while (retrievedItems.Count > 0)
			{
				int addedCount = 0;
				foreach (var item in retrievedItems)
				{
					if (ReachedMaxResults(items.Count))
						break;
					if (!filter.Match(item))
						continue;
					++addedCount;
					items.Add(item);
				}
				if (addedCount == retrievedItems.Count)
					break; // we've already added all items down there
				if (ReachedMaxResults(items.Count))
					break; // we've reached the items we need
				if (--maxRequeries == 0)
					break;
	
				// try finding more items in the database
				totalRetrievedItems += retrievedItems.Count;
				retrievedItems = CreateQuery(selectHql, totalRetrievedItems + FirstResult, MaxResults, Cachable).List<T>();
			}
			return items;
		}

		private bool ReachedMaxResults(int itemCount)
		{
			return MaxResults > 0 && itemCount >= MaxResults;
		}

		#endregion
	}
}