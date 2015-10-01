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
		public int Count;
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
			return Interlocked.Increment(ref Count);
		}
	}

	[Service]
	public class BucketFiller
	{
		ConcurrentDictionary<int, Bucket> buckets = new ConcurrentDictionary<int, Bucket>();

		public void RegisterDrip(Web.PathData data)
		{
			if (data.IsEmpty())
				return;

			var bucket = buckets.GetOrAdd(data.ID, CreateBucket);
			bucket.Increment();
		}

		public IEnumerable<Bucket> CheckoutBuckets()
		{
			var bs = buckets;
			buckets = new ConcurrentDictionary<int, Bucket>();
			return bs.Values;
		}

		private static Bucket CreateBucket(int id)
		{
			return new Bucket(id);
		}
	}
}