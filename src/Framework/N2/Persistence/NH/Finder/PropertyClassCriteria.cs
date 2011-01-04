using System;
using System.Collections.Generic;
using System.Text;
using N2.Persistence.Finder;

namespace N2.Persistence.NH.Finder
{
	/// <summary>
	/// The criteria building block of a query. Compares the item class to a value.
	/// </summary>
	public class PropertyClassCriteria : ICriteria<Type>
	{
		private readonly Operator op;
		private readonly QueryBuilder query;

		public PropertyClassCriteria(QueryBuilder query)
		{
			op = query.CurrentOperator;
			this.query = query;
		}

		#region ICriteria<Type> Members

		public IQueryAction Eq(Type value)
		{
			query.Criterias.Add(new PropertyHqlProvider<string>(op, "class", Comparison.Equal, query.GetDiscriminator(value)));
			return query;
		}

		public IQueryAction NotEq(Type value)
		{
			query.Criterias.Add(new PropertyHqlProvider<string>(op, "class", Comparison.NotEqual, query.GetDiscriminator(value)));
			return query;
		}

		public IQueryAction In(params Type[] values)
		{
			string[] discriminators = new string[values.Length];
			for (int i = 0; i < discriminators.Length; i++)
			{
				discriminators[i] = query.GetDiscriminator(values[i]);
			}
			query.Criterias.Add(new PropertyInHqlProvider<string>(op, "class", discriminators));
			return query;
		}
		#endregion
	}
}
