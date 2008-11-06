using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Tests.Web
{
	public class DataItem : N2.ContentItem
	{
		public override bool IsPage
		{
			get { return false; }
		}

		public override string TemplateUrl
		{
			get { return "~/Part.ascx"; }
		}
	}
}
