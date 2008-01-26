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

namespace N2.Templates.Faq.Items
{
	[Definition("Frequently Asked Questions", "FaqList", "A list of frequently asked questions with answers.", "", 200)]
	[AvailableZone("Questions", "Questions")]
	[RestrictParents(typeof(IStructuralPage))]
	public class FaqList : AbstractContentPage
	{

		[N2.Details.EditableChildren("Questions", "Questions", 110, ContainerName="content")]
		public virtual IList<Faq> Questions
		{
			get { return GetChildren<Faq>("Questions"); }
		}


		public override string IconUrl
		{
			get
			{
				return "~/Faq/UI/Img/help.png";
			}
		}
		public override string TemplateUrl
		{
			get
			{
				return "~/Faq/UI/Default.aspx";
			}
		}
	}
}
