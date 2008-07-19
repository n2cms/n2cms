using System;
using System.Web.UI;
namespace N2.Web.UI
{
    public interface ICacheManager
    {
        bool Enabled { get; }

        void AddCacheInvalidation(System.Web.UI.Page page);
        OutputCacheParameters GetOutputCacheParameters();
    }
}
