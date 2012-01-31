using System;
using NHibernate;
using System.Data;

namespace N2.Persistence.NH
{
	public class NHTransaction : ITransaction
	{
		NHibernate.ITransaction transaction;
        bool isOriginator = true;

		public NHTransaction(IsolationLevel? isolation, ISessionProvider sessionProvider)
		{
		    ISession session = sessionProvider.OpenSession.Session;
			transaction = session.Transaction;
			if (transaction.IsActive)
				isOriginator = false; // The method that first opened the transaction should also close it
			else if (isolation.HasValue)
				transaction.Begin(isolation.Value);
			else
                transaction.Begin();
		}

		#region ITransaction Members

		public void Commit()
		{
            if(isOriginator && !transaction.WasCommitted && !transaction.WasRolledBack)
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
			if(isOriginator)
			{
				Rollback();
				transaction.Dispose();
			}
			
		}

		#endregion
	}
}
