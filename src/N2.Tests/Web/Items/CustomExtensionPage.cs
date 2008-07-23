using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Tests.Web.Items
{
    public class CustomExtensionPage : PageItem
    {
        public static string extension = "/";
        public override string Extension
        {
            get { return extension; }
        }
    }
}
