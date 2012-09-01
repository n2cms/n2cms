using System.Web.UI;
using N2.Web;
using System;

namespace N2.Details
{
	[AttributeUsage(AttributeTargets.Property)]
	public class DisplayableAnchorAttribute : AbstractDisplayableAttribute, IDisplayable, IWritingDisplayable
	{
		private string target = null;
		
		public string Target
		{
			get { return target; }
			set { target = value; }
		}

		public override Control AddTo(ContentItem item, string detailName, Control container)
		{
			ContentItem linkedItem = item[detailName] as ContentItem;
			if (linkedItem != null)
				return GetLinkBuilder(item, linkedItem, detailName, Target, CssClass).AddTo(container);

			string url = item[detailName] as string;
			if (url != null)
				return GetLinkBuilder(item, url, detailName, Target, CssClass).AddTo(container);

			return null;
		}

		#region IWritingDisplayable Members

		public override void Write(ContentItem item, string detailName, System.IO.TextWriter writer)
		{
			ContentItem linkedItem = item[detailName] as ContentItem;
			if (linkedItem != null)
				GetLinkBuilder(item, linkedItem, detailName, Target, CssClass).WriteTo(writer);

			string url = item[detailName] as string;
			if (url != null)
				GetLinkBuilder(item, url, detailName, Target, CssClass).WriteTo(writer);
		}

		#endregion

		internal static ILinkBuilder GetLinkBuilder(ContentItem item, ContentItem linkedItem, string detailName, string target, string cssClass)
		{
			return Link.To(linkedItem)
				.Target(item.GetDetail(detailName + "_Target", target))
				.Class(item.GetDetail(detailName + "_CssClass", cssClass));
		}

		internal static ILinkBuilder GetLinkBuilder(ContentItem item, string url, string detailName, string target, string cssClass)
		{
			ILinkBuilder builder = new Link(item.GetDetail(detailName + "_Text", detailName), url);
			return builder.Target(item.GetDetail(detailName + "_Target", target))
				.Class(item.GetDetail(detailName + "_CssClass", cssClass));
		}
	}
}
