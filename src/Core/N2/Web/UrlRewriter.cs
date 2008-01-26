#region License

/* Copyright (C) 2007 Cristian Libardo
 *
 * This is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as
 * published by the Free Software Foundation; either version 2.1 of
 * the License, or (at your option) any later version.
 *
 * This software is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this software; if not, write to the Free
 * Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
 * 02110-1301 USA, or see the FSF site: http://www.fsf.org.
 */

#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Web;
using N2.Persistence;

namespace N2.Web
{
	/// <summary>
	/// Search crawler (and user) friendly url rewriter for N2 cms. The 
	/// rewriter walks the tree structure using the page's names to find the 
	/// appropriate name.
	/// </summary>
	public class UrlRewriter : IUrlRewriter
	{
		#region Private Fields

		private readonly IDictionary<string, string> rewritesCache = new Dictionary<string, string>();
		private readonly IUrlParser urlParser;

		#endregion

		#region Constructor

		/// <summary>Creates a new instance of the UrlRewriter.</summary>
		public UrlRewriter(IPersister persister, IUrlParser urlParser)
		{
			this.urlParser = urlParser;
			persister.ItemDeleted += ItemChangedEventHandler;
			persister.ItemMoved += ItemChangedEventHandler;
			persister.ItemSaved += ItemChangedEventHandler;
		}

		#endregion

		#region Properties

		/// <summary>The currently built rewrites cache.</summary>
		protected virtual IDictionary<string, string> RewritesCache
		{
			get { return rewritesCache; }
		}

		#endregion

		#region Rewrite Methods

		/// <summary>Rewrites a dynamic/computed url to an actual template url.</summary>
		/// <param name="context">The context to perform the rewriting on.</param>
		public virtual void RewriteRequest(IWebContext context)
		{
			if (IsRewritable(context.Request))
			{
				string requestedUrl = context.RelativeUrl;
				string rewrittenUrl;
				string key = context.Request.Url.Authority + requestedUrl;
				if (RewritesCache.ContainsKey(key))
				{
					rewrittenUrl = RewritesCache[key];
				}
				else
				{
					try
					{
						rewritesCache[key] = rewrittenUrl = GetRewrittenUrl(context);
					}
					catch (InvalidPathException ex)
					{
						Trace.TraceWarning(ex.ToString());
						return;
					}
				}

				if (string.IsNullOrEmpty(context.QueryString))
					context.RewritePath(rewrittenUrl);
				else
					context.RewritePath(rewrittenUrl + "&" + context.QueryString);
			}
		}

		/// <summary>Clears rewrites cache.</summary>
		public virtual void ClearRewrites()
		{
			rewritesCache.Clear();
		}

		protected virtual string GetRewrittenUrl(IWebContext context)
		{
			ContentItem item = urlParser.Parse(context.RelativeUrl);

			if (item == null)
				throw new InvalidPathException(context.RelativeUrl);

			return item.RewrittenUrl;
		}

		//protected virtual string AppendExistingQueryString(string url, IWebContext context)
		//{
		//    StringBuilder builder = new StringBuilder(url);
		//    foreach (string key in context.Request.QueryString.AllKeys)
		//    {
		//        if (!key.Equals("item", StringComparison.InvariantCultureIgnoreCase)
		//            || key.Equals("page", StringComparison.InvariantCultureIgnoreCase))
		//        {
		//            builder.AppendFormat("&{0}={1}", key, context.Request.QueryString[key]);
		//        }
		//    }
		//    return builder.ToString();
		//}

		protected virtual bool IsRewritable(HttpRequest request)
		{
			return request.Url.AbsolutePath.EndsWith(urlParser.DefaultExtension, StringComparison.InvariantCultureIgnoreCase)
			       && !File.Exists(request.PhysicalPath);
		}

		/// <summary>Is invoked when the site maps is changed. Clears rewrites.</summary>
		/// <param name="item">The item responsible of the change.</param>
		protected virtual void OnSiteMapChanged(ContentItem item)
		{
			ClearRewrites();
		}

		#endregion

		#region Event Handler Methods

		private void ItemChangedEventHandler(object sender, ItemEventArgs e)
		{
			OnSiteMapChanged(e.AffectedItem);
		}

		#endregion
	}
}