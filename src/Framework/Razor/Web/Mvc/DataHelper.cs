using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using N2;
using System.Dynamic;

namespace N2.Web.Mvc
{
    /// <remarks>This code is here since it has dependencies on ASP.NET 3.0 which isn't a requirement for N2 in general.</remarks>
    public class DataHelper : DynamicObject
    {
        Func<ContentItem> current;
        Dictionary<string, object> overrides = new Dictionary<string, object>();

        public DataHelper(Func<ContentItem> current)
        {
            this.current = current;
        }
        
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return current().GetContentType().GetProperties().Select(p => p.Name);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            string name = binder.Name;

            if (overrides.TryGetValue(name, out result))
            {
                return true;
            }

            var item = current();
            if (item == null)
            {
                result = null;
                return true;
            }

            result = item[name];
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            overrides[binder.Name] = value;
            return true;
        }
    }
}
