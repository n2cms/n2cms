using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence
{
    class FilesystemTransaction : ITransaction
    {
        public void Commit()
        {
			if (Committed != null)
				Committed(this, new EventArgs());
        }

        public void Rollback()
		{
			if (Rollbacked != null)
				Rollbacked(this, new EventArgs());
        }

        public event EventHandler Committed;

        public event EventHandler Rollbacked;

        public event EventHandler Disposed;

        public void Dispose()
		{
			if (Disposed != null)
				Disposed(this, new EventArgs());
        }
    }
}
