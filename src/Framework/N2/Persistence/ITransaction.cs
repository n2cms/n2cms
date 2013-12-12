using System;

namespace N2.Persistence
{
    /// <summary>
    /// Classes implementing this interface handle starting and stopping of 
    /// database transactions.
    /// </summary>
    public interface ITransaction : IDisposable
    {
        /// <summary>Commits the transaction.</summary>
        void Commit();

        /// <summary>Rollsbacks the transaction</summary>
        void Rollback();

        /// <summary>Invoked after the transaction has been committed.</summary>
        event EventHandler Committed;

        /// <summary>Invoked after the transaction has been rollbacked.</summary>
        event EventHandler Rollbacked;

        /// <summary>Invoked after the transaction has closed and is disposed.</summary>
        event EventHandler Disposed;
    }
}
