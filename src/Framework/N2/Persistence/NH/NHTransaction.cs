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
			var context = sessionProvider.OpenSession;
		    
			ISession session = context.Session;
			transaction = session.Transaction;
			if (transaction.IsActive)
				isOriginator = false; // The method that first opened the transaction should also close it
			else if (isolation.HasValue)
				transaction.Begin(isolation.Value);
			else
                transaction.Begin();

			if (context.Transaction != null)
			{
				context.Transaction.Committed += (o, s) => OnCommit();
				context.Transaction.Disposed += (o, s) => OnDispose();
				context.Transaction.Rollbacked += (o, s) => OnRollback();
			}
		}

		#region ITransaction Members

		/// <summary>Commits the transaction.</summary>
		public void Commit()
		{
			if (isOriginator && !transaction.WasCommitted && !transaction.WasRolledBack)
			{
				transaction.Commit();

				OnCommit();
			}
		}

		private void OnCommit()
		{
			if (Committed != null)
				Committed(this, new EventArgs());
		}

		/// <summary>Rollsbacks the transaction</summary>
		public void Rollback()
		{
			if (!transaction.WasCommitted && !transaction.WasRolledBack)
			{
				transaction.Rollback();

				OnRollback();
			}
		}

		private void OnRollback()
		{
			if (Rollbacked != null)
				Rollbacked(this, new EventArgs());
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
			OnDispose();
		}

		private void OnDispose()
		{
			if (Disposed != null)
				Disposed(this, new EventArgs());
		}

		#endregion

		/// <summary>Invoked after the transaction has been committed.</summary>
		public event EventHandler Committed;

		/// <summary>Invoked after the transaction has been rollbacked.</summary>
		public event EventHandler Rollbacked;

		/// <summary>Invoked after the transaction has closed and is disposed.</summary>
		public event EventHandler Disposed;
	}
}
