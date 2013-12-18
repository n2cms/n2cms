using System.Collections;
using N2.Web;

namespace N2.Tests.Fakes
{
    public class StaticContextAccessor : IRequestContextAccessor
    {
        Hashtable context = new Hashtable();

        public object Get(object key)
        {
            return context[key];
        }

        public void Set(object key, object instance)
        {
            context[key] = instance;
        }
    }
}
