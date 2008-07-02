using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using N2.Engine;

namespace N2.Templates.Wiki.Web
{
    public class WikiUserControl : UserControl, IWikiTemplate
    {
        public IEngine Engine
        {
            get { return N2.Context.Current; }
        }
        public RenderingContext WikiContext { get; set; }
    }
}
