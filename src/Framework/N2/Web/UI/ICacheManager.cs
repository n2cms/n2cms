using System;
using System.Web;
using System.Web.UI;

namespace N2.Web.UI
{
    public interface ICacheManager
    {
        bool Enabled { get; }

		void AddCacheInvalidation(ContentItem item, HttpResponse response);
		[Obsolete]
		void AddCacheInvalidation(HttpResponse response);
        OutputCacheParameters GetOutputCacheParameters();
	}
}
