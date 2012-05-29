using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2;
using N2.Definitions;
using N2.Details;

namespace N2.Management.Content.LinkTracker
{
	[PageDefinition(IconUrl = "{IconsUrl}/error_go.png",
		TemplateUrl = "{ManagementUrl}/Resources/RedirectHandler.ashx")]
	public class PermanentRedirect : ContentItem, IRedirect
	{
		public PermanentRedirect()
		{
			Visible = false;
		}

		public string RedirectUrl
		{
			get { return GetDetail<string>("RedirectUrl", null); }
			set { SetDetail("RedirectUrl", value); }
		}

		[EditableLink]
		public ContentItem RedirectTo
		{
			get { return GetDetail<ContentItem>("RedirectTo", null); }
			set { SetDetail("RedirectTo", value); }
		}
	}
}