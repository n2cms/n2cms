using System;
using N2.Templates.Rss.Items;
using N2.Templates.Syndication;
using N2.Web.UI;

namespace N2.Templates.Rss.UI
{
	public partial class Feed : Page<RssFeed>
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			if (Request["hungry"] == "yes")
			{
				Response.ContentType = "text/xml";
				Engine.Resolve<RssWriter>().Write(Response.Output, CurrentItem);
				Response.End();
			}
		}
	}
}