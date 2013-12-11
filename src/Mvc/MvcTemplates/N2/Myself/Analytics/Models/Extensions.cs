using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

namespace N2.Management.Myself.Analytics.Models
{
    public static class Extensions
    {
        public static string SplitWords(this string word)
        {
            return Regex.Replace(word, "([0-9]+|[A-Z]+)", (w) => " " + w.Value.ToLower());
        }
    }
}
