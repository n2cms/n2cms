using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Globalization
{
	public class Scope: IDisposable
	{
		Action end;

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
