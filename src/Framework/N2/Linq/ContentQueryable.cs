using System.Linq;
using System.Linq.Expressions;

namespace N2.Linq
{
	internal class ContentQueryable<T> : IQueryable<T>
	{
		readonly IQueryable<T> query;
		readonly ContentQueryProvider provider;

		public ContentQueryable(IQueryable<T> query)
		{
			provider = new ContentQueryProvider(query);
			this.query = query;
		}
		public ContentQueryable(ContentQueryProvider provider, IQueryable<T> query)
		{
			this.provider = provider;
			this.query = query;
		}

		#region IEnumerable<T> Members

		public System.Collections.Generic.IEnumerator<T> GetEnumerator()
		{
			return query.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		#region IQueryable Members

		public System.Type ElementType
		{
			get { return query.ElementType; }
		}

		public Expression Expression
		{
			get { return query.Expression; }
		}

		public IQueryProvider Provider
		{
			get { return provider; }
		}

		#endregion
	}

}
