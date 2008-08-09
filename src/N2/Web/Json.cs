using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;

namespace N2.Web
{
    /// <summary>
    /// Json helper methods.
    /// </summary>
    public static class Json
    {
        public static string ToObject(NameValueCollection collection)
        {
            if (collection.Count == 0)
                return "{}";

            StringBuilder sb = new StringBuilder("{");
        
            foreach (string key in collection.Keys)
            {
                sb.Append(key).Append(":'").Append(collection[key]).Append("',");
            }
            sb.Length--; // remove trailing comma
            return sb.Append("}").ToString();
        }
    }
}
