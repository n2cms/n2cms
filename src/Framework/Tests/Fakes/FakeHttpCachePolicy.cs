using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace N2.Tests.Fakes
{
	public class FakeHttpCachePolicy : HttpCachePolicyBase
	{
		public HttpCacheability cacheability;
		public HttpCacheRevalidation revalidation;
		public DateTime date;
		public TimeSpan delta;

		public override void SetCacheability(HttpCacheability cacheability)
		{
			this.cacheability = cacheability;
		}

		public override void SetRevalidation(HttpCacheRevalidation revalidation)
		{
			this.revalidation = revalidation;
		}

		public override void SetLastModified(DateTime date)
		{
			this.date = date;
		}

		public override void SetMaxAge(TimeSpan delta)
		{
			this.delta = delta;
		}
	}
}
