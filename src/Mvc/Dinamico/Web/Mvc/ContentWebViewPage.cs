using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Designerific.Web.Mvc
{
	public abstract class ContentWebViewPage : WebViewPage
	{
	}
	public abstract class ContentWebViewPage<TModel> : WebViewPage<TModel>
	{
	}
}