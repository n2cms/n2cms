using System;
using System.Collections.Generic;
using System.Text;
using N2.Collections;
using N2.Definitions;
using N2.Persistence.Finder;
using NHibernate;

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

		private readonly IDefinitionManager definitions;
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

		public QueryBuilder(ISessionProvider sessionProvider, IDefinitionManager definitions)
		{
			this.sessionProvider = sessionProvider;
			this.definitions = definitions;
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

		public IComparisonCriteria<DateTime> Published
		{
			get { return new PropertyCriteria<DateTime>(this, "Published"); }
		}

		public IComparisonCriteria<DateTime> Expires
		{
			get { return new PropertyCriteria<DateTime>(this, "Expires"); }
		}

		public IComparisonCriteria<int> SortOrder
		{
			get { return new PropertyCriteria<int>(this, "SortOrder"); }
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

		#endregion

		#region Methods

		public string GetDiscriminator(Type value)
		{
			if (value == null) throw new ArgumentNullException("value");

			ItemDefinition definition = definitions.GetDefinition(value);
			if(definition == null)
				throw new ArgumentException("Could not find the definition associated with the type '" + value.FullName + "'. Please ensure this is a non-abstract class deriving from N2.ContentItem and that it is decorated by the [Definition] attribute.");
			return definition.Discriminator;
		}

		protected virtual IQuery CreateQuery()
		{
			return CreateQuery("select ci from ContentItem ci");
		}

		protected virtual IQuery CreateQuery(string selectFrom)
		{
			if (MaxResults > 0 && Filters.Count > 0)
				throw new N2Exception("Cannot use filters when using MaxResults, sorry.");
			if (FirstResult > 0 && Filters.Count > 0)
				throw new N2Exception("Cannot use filters when using FirstResult, sorry.");

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

			if (MaxResults > 0)
				query.SetMaxResults(MaxResults);
			if (FirstResult > 0)
				query.SetFirstResult(FirstResult);
			query.SetCacheable(Cachable);

			return query;
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

		public int Count()
		{
			if (Filters.Count > 0)
				throw new N2Exception("Cannot use filters when selecting count, sorry.");

			IQuery q = CreateQuery("select count(*) from ContentItem ci");
			return Convert.ToInt32(q.List()[0]);
		}

		public IList<ContentItem> Select()
		{
			return Select<ContentItem>();
		}

		public IList<T> Select<T>() where T : ContentItem
		{
            ItemList<T> items = new ItemList<T>(
                CreateQuery().Enumerable<T>(), 
                new CompositeFilter(Filters ?? new ItemFilter[0]));

			if (SortExpression != null)
				items.Sort(SortExpression);

			return items;
		}

		#endregion
	}
}