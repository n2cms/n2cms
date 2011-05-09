using System.Text;

namespace N2.Persistence.NH.Finder
{
	/// <summary>
	/// Creates the hql query for finding versions of an item.
	/// </summary>
	public class VersionOfHqlProvider : IHqlProvider
	{
		Operator op;
		int itemID;

		public VersionOfHqlProvider(Operator op, ContentItem item)
		{
			this.op=op;
			this.itemID = item.ID;
		}

		#region IHqlProvider Members

		public void AppendHql(StringBuilder from, StringBuilder where, int index)
		{
			where.AppendFormat(" {0} VersionOfID = :v{1}",
				GetOperator(),
				index);
		}

		public void SetParameters(NHibernate.IQuery query, int index)
		{
			query.SetParameter("v" + index, this.itemID);
		}

		#endregion

		protected virtual string GetOperator()
		{
			if (op == Operator.None)
				return string.Empty;
			else
				return op.ToString();
		}
	}
}
