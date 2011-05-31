using System.Collections.Generic;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web;
using System.Web.Routing;
using N2.Web;
using N2.Web.Mvc;

namespace N2.Engine.Providers
{
	[Service(typeof(IProvider<ContentHelper>))]
	public class ContentHelperProvider : IProvider<ContentHelper>
	{
		IProvider<HtmlHelper> htmlHelperProvider;

		public ContentHelperProvider(IProvider<HtmlHelper> htmlHelperProvider)
		{
			this.htmlHelperProvider = htmlHelperProvider;
		}

		#region IProvider<ContentHelper> Members

		public ContentHelper Get()
		{
			return new ContentHelper(htmlHelperProvider.Get());
		}

		public IEnumerable<ContentHelper> GetAll()
		{
			return new[] { Get() };
		}

		#endregion
	}
}
