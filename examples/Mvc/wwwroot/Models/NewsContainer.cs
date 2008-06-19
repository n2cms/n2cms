using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using N2;
using System.Collections.Generic;
using N2.Collections;

namespace MvcTest.Models
{
	[Definition("News Collection")]
	public class NewsContainer : AbstractPage
	{
		public virtual IEnumerable<NewsPage> GetNews()
		{
			return N2.Find.OfType<NewsPage>(GetChildren(new AccessFilter(), new TypeFilter(typeof(NewsPage))));
		}
	}
}
