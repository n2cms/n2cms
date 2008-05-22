using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Persistence
{
	public interface ITransaction : IDisposable
	{
		void Commit();
		void Rollback();
	}
}
