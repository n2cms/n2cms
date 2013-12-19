using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Web;

namespace N2.Tests.Fakes
{
    public class FakeCacheWrapper : CacheWrapper
    {
        public FakeCacheWrapper()
            : base(null, null, new N2.Configuration.DatabaseSection())
        {
        }

        Dictionary<string, object> cache = new Dictionary<string, object>();

        public override T Get<T>(string cacheKey)
        {
            if (cache.ContainsKey(cacheKey))
                return (T)cache[cacheKey];
            return null;
        }
        public override void Add(string cacheKey, object value, CacheOptions options = null)
        {
            cache[cacheKey] = value;
        }
        public override object Get(string cacheKey)
        {
            return Get<object>(cacheKey);
        }
        public override System.Web.Caching.CacheDependency GetCacheDependency(CacheOptions options)
        {
            return base.GetCacheDependency(options);
        }
        public override T GetOrCreate<T>(string cacheKey, Func<T> factory, CacheOptions options = null)
        {
            return Get<T>(cacheKey) ?? factory();
        }
        public override void Remove(string cacheKey)
        {
            cache.Remove(cacheKey);
        }
    }
}
