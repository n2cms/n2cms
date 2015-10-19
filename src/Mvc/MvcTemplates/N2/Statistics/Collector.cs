using N2.Engine;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Management.Statistics
{
	[Service]
	public class Collector
	{
		ConcurrentDictionary<int, Bucket> buckets = new ConcurrentDictionary<int, Bucket>();

		public void RegisterView(Web.PathData data)
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
