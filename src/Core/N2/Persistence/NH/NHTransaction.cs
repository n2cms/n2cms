using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Persistence.NH
{
	public class NHTransaction : ITransaction
	{
		NHibernate.ITransaction transaction;

		public NHTransaction(ISessionProvider sessionProvider)
		{
			transaction = sessionProvider.GetOpenedSession().BeginTransaction();
		}

		#region ITransaction Members

		public void Commit()
		{
			transaction.Commit();
		}

		public void Rollback()
		{
			transaction.Rollback();
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			if (!transaction.WasCommitted && !transaction.WasRolledBack)
				Rollback();
			transaction.Dispose();
		}

		#endregion
	}
}
