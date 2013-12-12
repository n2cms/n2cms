using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Addons.Wiki
{
    public static class Utility
    {
        public static string CapitalizeFirstLetter(string name)
        {
            if (string.IsNullOrEmpty(name))
                return name;
            return name[0].ToString().ToUpper() + name.Substring(1);
        }    
    }
}
