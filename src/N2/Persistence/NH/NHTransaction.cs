using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;

namespace N2.Persistence.NH
{
	public class NHTransaction : ITransaction
	{
		NHibernate.ITransaction transaction;

		public NHTransaction(ISessionProvider sessionProvider)
		{
            ISession session = sessionProvider.GetOpenedSession();
			transaction = session.Transaction;
            if (!transaction.IsActive)
                transaction.Begin();
		}

		#region ITransaction Members

		public void Commit()
		{
            if(!transaction.WasCommitted && !transaction.WasRolledBack)
			    transaction.Commit();
		}

		public void Rollback()
		{
            if (!transaction.WasCommitted && !transaction.WasRolledBack)
                transaction.Rollback();
		}

		#endregion

		#region IDisposable Members

        void IDisposable.Dispose()
		{
			Rollback();
			transaction.Dispose();
		}

		#endregion
	}
}
