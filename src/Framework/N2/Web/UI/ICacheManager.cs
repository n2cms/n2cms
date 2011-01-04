using System;
using System.Web.UI;
using System.Web;
namespace N2.Web.UI
{
    public interface ICacheManager
    {
        bool Enabled { get; }

        void AddCacheInvalidation(HttpResponse response);
        OutputCacheParameters GetOutputCacheParameters();
    }
}
