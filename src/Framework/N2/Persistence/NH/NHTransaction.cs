using System;
using NHibernate;
using System.Data;
using System.Diagnostics;
using N2.Engine;

namespace N2.Persistence.NH
{
    public class NHTransaction : ITransaction
    {
        Logger<NHTransaction> logger;
        NHibernate.ITransaction transaction;
        private SessionContext context;
        bool isOriginator = true;

        public bool IsCommitted { get; set; }
        public bool IsRollbacked { get; set; }

        public NHTransaction(ISessionProvider sessionProvider)
        {
            context = sessionProvider.OpenSession;
            
            ISession session = context.Session;
            transaction = session.Transaction;
            if (transaction.IsActive)
                isOriginator = false; // The method that first opened the transaction should also close it
            else if (sessionProvider.Isolation.HasValue)
                transaction.Begin(sessionProvider.Isolation.Value);
            else
                transaction.Begin();

            logger.InfoFormat("Begin {0}, transaction:{1}, isOriginator:{2}, isolation:{3} [", GetHashCode(), transaction.GetHashCode(), isOriginator, sessionProvider.Isolation);
            logger.Indent();

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
            if (isOriginator)
            {
				if (!transaction.WasCommitted && !transaction.WasRolledBack)
				{
					logger.InfoFormat("Committing {0}", transaction.GetHashCode());
					transaction.Commit();
					IsCommitted = true;
					RemoveFromContext();

					OnCommit();
				}
				else
				{
					logger.WarnFormat("Not commiting {0}, isOriginator:{1}, wasCommitted:{2}, wasRolledBack:{3}", transaction.GetHashCode(), isOriginator, transaction.WasCommitted, transaction.WasRolledBack);
				}
            }
            else
            {
                logger.InfoFormat("Not commiting {0}, isOriginator:{1}, wasCommitted:{2}, wasRolledBack:{3}", transaction.GetHashCode(), isOriginator, transaction.WasCommitted, transaction.WasRolledBack);
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
                logger.WarnFormat("Rollback {0}", transaction.GetHashCode());
                transaction.Rollback();
                IsRollbacked = true;
                RemoveFromContext();

                OnRollback();
            }
            else if (!transaction.WasCommitted)
            {
                logger.WarnFormat("Not rollbacking {0}, wasCommitted:{1}, wasRolledBack:{2}", transaction.GetHashCode(), transaction.WasCommitted, transaction.WasRolledBack);
            }
        }

        private void OnRollback()
        {
            if (Rollbacked != null)
                Rollbacked(this, new EventArgs());
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            logger.Unindent();
            if (isOriginator)
            {
				if (!transaction.WasCommitted)
					Rollback();
                transaction.Dispose();
                RemoveFromContext();
            }
            logger.Info("]");

            OnDispose();
        }

        private void OnDispose()
        {
            if (Disposed != null)
                Disposed(this, new EventArgs());
        }

        #endregion

        private void RemoveFromContext()
        {
            if (object.ReferenceEquals(context.Transaction, this))
            {
                logger.DebugFormat("Removing context transaction {0}", this.GetHashCode());
                context.Transaction = null;
            }
        }

        /// <summary>Invoked after the transaction has been committed.</summary>
        public event EventHandler Committed;

        /// <summary>Invoked after the transaction has been rollbacked.</summary>
        public event EventHandler Rollbacked;

        /// <summary>Invoked after the transaction has closed and is disposed.</summary>
        public event EventHandler Disposed;
    }
}
