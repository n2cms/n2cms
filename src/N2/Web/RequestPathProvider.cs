using System;
using System.Collections.Specialized;
using N2.Engine;
using N2.Configuration;
using System.Collections.Generic;

namespace N2.Web
{
	/// <summary>
	/// Resolves the controller to handle a certain request. Supports a default 
	/// controller or additional imprativly using the ConnectControllers method 
	/// or declarativly using the [Controls] attribute registered.
	/// </summary>
	public class RequestPathProvider
	{
		readonly IContentAdapterProvider adapterProvider;
		readonly IWebContext webContext;
		readonly IUrlParser parser;
		readonly IErrorHandler errorHandler;
		readonly bool rewriteEmptyExtension = true;
		readonly bool observeAllExtensions = true;
		readonly string[] observedExtensions = new[] { ".aspx" };
		readonly string[] nonRewritablePaths = new[] {"~/N2/Content/"};

		public RequestPathProvider(IContentAdapterProvider adapterProvider, IWebContext webContext, IUrlParser parser, IErrorHandler errorHandler, HostSection config)
		{
			this.adapterProvider = adapterProvider;
			this.webContext = webContext;
			this.parser = parser;
			this.errorHandler = errorHandler;
			observeAllExtensions = config.Web.ObserveAllExtensions;
			rewriteEmptyExtension = config.Web.ObserveEmptyExtension;
			StringCollection additionalExtensions = config.Web.ObservedExtensions;
            if (additionalExtensions != null && additionalExtensions.Count > 0)
            {
                observedExtensions = new string[additionalExtensions.Count + 1];
                additionalExtensions.CopyTo(observedExtensions, 1);
            }
			observedExtensions[0] = config.Web.Extension;
			nonRewritablePaths = config.Web.Urls.NonRewritable.GetPaths(webContext);
		}


		public virtual PathData GetCurrentPath()
		{
			Url url = webContext.Url;
			string path = url.Path;
			foreach (string nonRewritablePath in nonRewritablePaths)
			{
				if (path.StartsWith(nonRewritablePath))
					return null;
			}

			PathData data = ResolveUrl(url);
			return data;
		}

		public virtual PathData ResolveUrl(string url)
		{
			try
			{
				if (IsObservable(url)) return parser.ResolvePath(url);
			}
			catch (Exception ex)
			{
				errorHandler.Notify(ex);
			}
			return PathData.Empty;
		}

		private bool IsObservable(Url url)
		{
			if (observeAllExtensions)
				return true;

			if(url.LocalUrl == Url.ApplicationPath)
				return true;

			string extension = url.Extension;
			if (rewriteEmptyExtension && string.IsNullOrEmpty(extension))
				return true;
			foreach (string observed in observedExtensions)
				if (string.Equals(observed, extension, StringComparison.InvariantCultureIgnoreCase))
					return true;
			if (url.GetQuery(PathData.PageQueryKey) != null)
				return true;

			return false;
		}
	}
}
