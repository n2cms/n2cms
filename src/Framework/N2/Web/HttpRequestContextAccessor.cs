using System.Web;

namespace N2.Web
{
    public class HttpRequestContextAccessor : IRequestContextAccessor
    {
        public object Get(object key)
        {
            return HttpContext.Current.Items[key];
        }

        public void Set(object key, object instance)
        {
            HttpContext.Current.Items[key] = instance;
        }
    }
}
