using System;

namespace N2.Persistence
{
    /// <summary>
    /// Classes implementing this interface handle starting and stopping of 
    /// database transactions.
    /// </summary>
	public interface ITransaction : IDisposable
	{
		void Commit();
		void Rollback();
	}
}
