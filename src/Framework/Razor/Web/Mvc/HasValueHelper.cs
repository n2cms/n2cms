using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using N2;
using System.Dynamic;

namespace N2.Web.Mvc
{
    /// <summary>
    /// Dynamic (short) access to the HasValue method that checks the presence for a certain value.
    /// </summary>
    /// <remarks>This code is here since it has dependencies on ASP.NET 3.0 which isn't a requirement for N2 in general.</remarks>
    public class HasValueHelper : DynamicObject
    {
        Func<string, bool> hasValue;

        public HasValueHelper(Func<string, bool> hasValue)
        {
            this.hasValue = hasValue;
        }
        
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            yield break;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = hasValue(binder.Name);
            return true;
        }
    }
}
