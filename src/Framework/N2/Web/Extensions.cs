using System.Web.Mvc;
using System.Collections.Generic;
using System.Web.UI;
using N2.Collections;

namespace N2.Web
{
	public static class Extensions
	{
		/// <summary>Converts a string to an <see cref="Url"/></summary>
		/// <param name="url">The url string.</param>
		/// <returns>The string parsed into an Url.</returns>
		public static Url ToUrl(this string url)
		{
			return Url.Parse(url);
		}


		public static Tree OpenTo(this Tree treeBuilder, ContentItem item)
		{
			IList<ContentItem> items = Find.ListParents(item);
			return treeBuilder.ClassProvider(null, n => items.Contains(n.Current) || n.Current == item ? "open" : string.Empty);
		}


		internal static TagBuilder AddAttributeUnlessEmpty(this TagBuilder tag, string attribute, string value)
		{
			if (string.IsNullOrEmpty(value))
				return tag;

			tag.Attributes[attribute] = value;

			return tag;
		}

		internal static Control AddTo(this ILinkBuilder builder, Control container)
		{
			var c = builder.ToControl();
			if (c != null)
				container.Controls.Add(c);
			return c;
		}
	}
}
