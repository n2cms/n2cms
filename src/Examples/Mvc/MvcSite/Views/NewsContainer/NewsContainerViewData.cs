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
using System.Collections.Generic;
using MvcTest.Models;

namespace MvcTest.Views.NewsContainer
{
	public class NewsContainerViewData
	{
		public Models.NewsContainer Container { get; set; }
		public IEnumerable<NewsPage> News { get; set; }
	}
}
