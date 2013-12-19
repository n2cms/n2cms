using System;

namespace N2.Engine.Globalization
{
    /// <summary>
    /// Executs an action upon disposal.
    /// </summary>
    public class Scope: IDisposable
    {
        Action end;

        /// <summary>Creates a new isntance of the scope.</summary>
        /// <param name="end">The action to execute upon object disposal.</param>
        public Scope(Action end)
        {
            this.end = end;
        }

        /// <summary>Creates a new isntance of the scope.</summary>
        /// <param name="begin">The action to execute immediately.</param>
        /// <param name="end">The action to execute upon object disposal.</param>
        public Scope(Action begin, Action end)
        {
            begin();
            this.end = end;
        }

        public void End()
        {
            end();
        }

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            End();
        }

        #endregion
    }
}
