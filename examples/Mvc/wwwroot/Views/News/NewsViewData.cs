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
using MvcTest.Models;
using System.Collections.Generic;
using N2;

namespace MvcTest.Views.News
{
	public class NewsViewData
	{
		public ContentItem Back { get; set; }
		public NewsPage News { get; set; }
		public IEnumerable<CommentItem> Comments { get; set; }
	}
}
