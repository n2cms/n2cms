using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine;

namespace N2.Tests.Fakes
{
	public class FakeWorker : IWorker
	{
		public int ExecutingWorkItems
		{
			get { throw new NotImplementedException(); }
		}

		public void DoWork(Action action)
		{
			action();
		}
	}
}
