using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Definitions;

namespace N2.Tests.Web.Items
{
    public class UrlSourcePage : ContentItem, IUrlSource
    {
        public string DirectUrl { get; set; }
    }
}
