using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using N2.Engine;
using N2.Security;

namespace N2.Web.Mvc
{
	/// <summary>
	/// Base class for content controllers that provides easy access to the content item in scope.
	/// </summary>
	public abstract class ContentController : ContentController<ContentItem>
	{
	}
}