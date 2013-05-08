using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using N2.Configuration;

namespace N2.Web
{
	public static class DocViewerUtility
	{
		public const string UseDocViewerPropertyKey = "UseDocViewer";
		public const string DocViewerUrlPropertyKey = "DocViewerUrl";

		/// <summary>
		/// Determines if a ContentItem is configured to use a custom viewer.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public static bool UseDocViewer(ContentItem model)
		{
			var useViewer = model.GetDetail(UseDocViewerPropertyKey, false);
			if (!useViewer)
			{
				// check site-wide configuration
				useViewer = Content.Traverse.StartPage.GetDetail(UseDocViewerPropertyKey, false);
				if (!useViewer)
				{
					// check web.config configuration
					if (CurrentDocViewerElement != null)
						return CurrentDocViewerElement.Enabled;
				}
			}
			return useViewer;
		}

		public static string GetDocViewerUrlTemplate(ContentItem model)
		{
			var url = model.GetDetail(DocViewerUrlPropertyKey, "");
			if (string.IsNullOrEmpty(url))
			{
				url = Content.Traverse.StartPage.GetDetail(DocViewerUrlPropertyKey, "");
				if (String.IsNullOrEmpty(url))
				{
					// check web.config configuration
					if (CurrentDocViewerElement != null)
						return CurrentDocViewerElement.Url;
				}
			}
			return url;
		}

		private static DocViewerElement _dve = null;
		private static DocViewerElement CurrentDocViewerElement
		{
			get { return (_dve = _dve ?? Context.Current.Resolve<DocViewerElement>());}
		}

		public static bool DocViewerAppliesTo(string documentUrl)
		{
			// check web.config configuration
			if (CurrentDocViewerElement == null)
				return false;

			// TODO: Check file size when verifying document viewer applicability.

			return CurrentDocViewerElement.FileExtensionsArray.Any(f => documentUrl.EndsWith(f, StringComparison.OrdinalIgnoreCase));
		}

		public static string GetDocumentUrl(ContentItem model, string documentUrl)
		{
			if (!UseDocViewer(model) || !DocViewerAppliesTo(documentUrl))
				return documentUrl;

			var url = GetDocViewerUrlTemplate(model);

			if (string.IsNullOrEmpty(url))
				return documentUrl;

			var resultUrl = new Uri(HttpContext.Current.Request.Url, documentUrl);
			var absUrl = resultUrl.GetComponents(UriComponents.HttpRequestUrl, UriFormat.UriEscaped);
			var relUrl = resultUrl.GetComponents(UriComponents.PathAndQuery, UriFormat.UriEscaped);
			var absEnc = HttpUtility.UrlEncode(absUrl);
			var relEnc = HttpUtility.UrlEncode(relUrl);

			// TODO: Use a nicer method of tokenizing strings. 
			// TODO: Use Url.ResolveTokens instead of String.Replace.
			return url
				.Replace("{abs}", absUrl)
				.Replace("{rel}", relUrl)
				.Replace("{abs_enc}", absEnc)
				.Replace("{rel_enc}", relEnc);
		}



	}
}
