using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using N2.Definitions;
using N2.Web;

namespace N2.Details
{
	public class DisplayableAnchorAttribute : AbstractDisplayableAttribute, IDisplayable
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
			{
				return AddAnchor(container, linkedItem, Target, CssClass);
			}
			return null;
		}

		public static Control AddAnchor(Control container, ContentItem linkedItem)
		{
			return AddAnchor(container, linkedItem, null, null);
		}

		public static Control AddAnchor(Control container, ContentItem linkedItem, string target, string cssClass)
		{
			Control anchor = Link.To(linkedItem).Target(target).Class(cssClass).ToControl();
			container.Controls.Add(anchor);
			return anchor;
		}
	}
}
