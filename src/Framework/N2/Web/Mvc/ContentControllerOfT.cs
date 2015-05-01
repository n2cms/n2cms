using System;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using N2.Engine;
using N2.Engine.Globalization;
using N2.Security;

namespace N2.Web.Mvc
{
    /// <summary>
    /// Base class for content controllers that provides easy access to the content item in scope.
    /// </summary>
    /// <typeparam name="T">The type of content item the controller handles.</typeparam>
	[ContentOutputCache]
	public abstract class ContentController<T> : ContentController
        where T : ContentItem
    {
        /// <summary>The content item associated with the requested path.</summary>
        public new virtual T CurrentItem
        {
            get { return base.CurrentItem as T ?? base.CurrentPage as T; }
            set { base.CurrentItem = value; }
        }

    }
}
