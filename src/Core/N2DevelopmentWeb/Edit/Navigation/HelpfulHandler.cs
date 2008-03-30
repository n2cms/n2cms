using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Specialized;

namespace N2.Edit.Navigation
{
	public abstract class HelpfulHandler : IHttpHandler
	{
		public virtual bool IsReusable
		{
			get { return true; }
		}

		public abstract void ProcessRequest(HttpContext context);

		protected ContentItem GetSelectedItem(NameValueCollection queryString)
		{
			string path = queryString["selected"];
			return N2.Context.Current.Resolve<N2.Edit.Navigator>().Navigate(path);
		}
	}
}
