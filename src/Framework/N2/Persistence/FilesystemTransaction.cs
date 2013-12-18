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
            //throw new NotImplementedException();
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public event EventHandler Committed;

        public event EventHandler Rollbacked;

        public event EventHandler Disposed;

        public void Dispose()
        {
        }
    }
}
