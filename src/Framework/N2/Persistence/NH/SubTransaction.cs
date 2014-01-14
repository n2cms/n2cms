using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace N2.Persistence.NH
{
    public class SubTransaction : ITransaction
    {
        private ITransaction parentTransaction;

        public SubTransaction(ITransaction parentTransaction)
        {
            this.parentTransaction = parentTransaction;
            Debug.WriteLine("Subtransaction {");
            Debug.Indent();
        }

        public void Commit()
        {
        }

        public void Rollback()
        {
            parentTransaction.Rollback();
        }

        public void Dispose()
        {
            Debug.Unindent();
            Debug.WriteLine("}");
        }

        public event EventHandler Committed
        {
            add { parentTransaction.Committed += value; }
            remove { parentTransaction.Committed -= value; }
        }

        public event EventHandler Rollbacked
        {
            add { parentTransaction.Rollbacked += value; }
            remove { parentTransaction.Rollbacked -= value; }
        }

        public event EventHandler Disposed
        {
            add { parentTransaction.Disposed += value; }
            remove { parentTransaction.Disposed -= value; }
        }
    }
}
