using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using N2.Integrity;
using System.Collections.Generic;
using N2.Templates.Items;

namespace N2.Templates.Items
{
    [Definition("Frequently Asked Questions", "FaqList", "A list of frequently asked questions with answers.", "", 200)]
    [AvailableZone("Questions", "Questions")]
    [RestrictParents(typeof(IStructuralPage))]
	[DefaultTemplate("FaqList")]
    public class FaqList : AbstractContentPage, IStructuralPage
    {
        [N2.Details.EditableChildren("Questions", "Questions", 110, ContainerName=Tabs.Content)]
        public virtual IList<Faq> Questions
        {
            get { return GetChildren<Faq>("Questions"); }
        }

        protected override string IconName
        {
            get { return "help"; }
        }
    }
}