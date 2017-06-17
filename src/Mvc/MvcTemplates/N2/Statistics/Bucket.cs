using N2.Engine;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace N2.Management.Statistics
{
	public class Bucket
	{
		public long ID;
		public int PageID;
		public int Views;
		public DateTime TimeSlot;
		
		public Bucket()
		{
		}
		
		public Bucket(int id)
		{
			this.PageID = id;
			TimeSlot = Utility.CurrentTime();
		}

		public int Increment()
		{
			return Interlocked.Increment(ref Views);
		}
	}
}