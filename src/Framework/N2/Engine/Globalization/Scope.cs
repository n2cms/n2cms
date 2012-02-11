using System;

namespace N2.Engine.Globalization
{
	public class Scope: IDisposable
	{
		Action end;

		public Scope(Action end)
		{
			this.end = end;
		}

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
